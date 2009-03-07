#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Activation;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Planning.Targets
{
	/// <summary>
	/// Represents a site on a type where a value can be injected.
	/// </summary>
	/// <typeparam name="T">The type of site this represents.</typeparam>
	public abstract class Target<T> : ITarget
		where T : ICustomAttributeProvider
	{
		private Func<IBindingMetadata, bool> _constraint;

		/// <summary>
		/// Gets the member that contains the target.
		/// </summary>
		public MemberInfo Member { get; private set; }

		/// <summary>
		/// Gets or sets the site (property, parameter, etc.) represented by the target.
		/// </summary>
		public T Site { get; private set; }

		/// <summary>
		/// Gets the name of the target.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the type of the target.
		/// </summary>
		public abstract Type Type { get; }

		/// <summary>
		/// Gets the constraint defined on the target.
		/// </summary>
		public Func<IBindingMetadata, bool> Constraint
		{
			get
			{
				if (_constraint == null) _constraint = ReadConstraintFromAttributes();
				return _constraint;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the target represents an optional dependency.
		/// </summary>
		public bool IsOptional
		{
			get { return Site.HasAttribute<OptionalAttribute>(); }
		}

			/// <summary>
		/// Initializes a new instance of the <see cref="Target&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="member">The member that contains the target.</param>
		/// <param name="site">The site represented by the target.</param>
		protected Target(MemberInfo member, T site)
		{
			Member = member;
			Site = site;
		}

		/// <summary>
		/// Returns an array of custom attributes of a specified type defined on the target.
		/// </summary>
		/// <param name="attributeType">The type of attribute to search for.</param>
		/// <param name="inherit">Whether to look up the hierarchy chain for inherited custom attributes.</param>
		/// <returns>An array of custom attributes of the specified type.</returns>
		public object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return Site.GetCustomAttributes(attributeType, inherit);
		}

		/// <summary>
		/// Returns an array of custom attributes defined on the target.
		/// </summary>
		/// <param name="inherit">Whether to look up the hierarchy chain for inherited custom attributes.</param>
		/// <returns>An array of custom attributes.</returns>
		public object[] GetCustomAttributes(bool inherit)
		{
			return Site.GetCustomAttributes(inherit);
		}

		/// <summary>
		/// Returns a value indicating whether an attribute of the specified type is defined on the target.
		/// </summary>
		/// <param name="attributeType">The type of attribute to search for.</param>
		/// <param name="inherit">Whether to look up the hierarchy chain for inherited custom attributes.</param>
		/// <returns><c>True</c> if such an attribute is defined; otherwise <c>false</c>.</returns>
		public bool IsDefined(Type attributeType, bool inherit)
		{
			return Site.IsDefined(attributeType, inherit);
		}

		/// <summary>
		/// Resolves a value for the target within the specified parent context.
		/// </summary>
		/// <param name="parent">The parent context.</param>
		/// <returns>The resolved value.</returns>
		public object ResolveWithin(IContext parent)
		{
			if (Type.IsArray)
			{
				Type service = Type.GetElementType();
				return ResolveInstances(service, parent).CastSlow(service).ToArraySlow(service);
			}

			if (Type.IsGenericType)
			{
				Type gtd = Type.GetGenericTypeDefinition();
				Type service = Type.GetGenericArguments()[0];

				if (gtd == typeof(List<>) || gtd == typeof(IList<>))
					return ResolveInstances(service, parent).CastSlow(service).ToListSlow(service);

				if (typeof(IEnumerable<>).IsAssignableFrom(gtd))
					return ResolveInstances(service, parent).CastSlow(service);
			}

			return ResolveInstances(Type, parent).FirstOrDefault();
		}

		private IEnumerable<object> ResolveInstances(Type service, IContext parent)
		{
			var request = parent.Request.CreateChild(service, this);
			return parent.Kernel.Resolve(request);
		}

		private Func<IBindingMetadata, bool> ReadConstraintFromAttributes()
		{
			ConstraintAttribute[] attributes = Site.GetAttributes<ConstraintAttribute>().ToArray();

			if (attributes.Length == 0) return null;
			if (attributes.Length == 1) return attributes[0].Matches;

			return metadata => attributes.All(attribute => attribute.Matches(metadata));
		}
	}
}