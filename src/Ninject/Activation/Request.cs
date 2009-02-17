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
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Activation
{
	/// <summary>
	/// Describes the request for a service resolution.
	/// </summary>
	public class Request : IRequest
	{
		/// <summary>
		/// Gets the service that was requested.
		/// </summary>
		public Type Service { get; private set; }

		/// <summary>
		/// Gets the parent request.
		/// </summary>
		public IRequest Parent { get; private set; }

		/// <summary>
		/// Gets the target that will receive the injection, if any.
		/// </summary>
		public ITarget Target { get; private set; }

		/// <summary>
		/// Gets the constraint that will be applied to filter the bindings used for the request.
		/// </summary>
		public Func<IBindingMetadata, bool> Constraint { get; private set; }

		/// <summary>
		/// Gets the parameters that affect the resolution.
		/// </summary>
		public ICollection<IParameter> Parameters { get; private set; }

		/// <summary>
		/// Gets the collection of bindings which have been activated either by this request or
		/// one of its ancestors.
		/// </summary>
		public ICollection<IBinding> ActiveBindings { get; private set; }

		/// <summary>
		/// Gets the callback that resolves the scope for the request, if an external scope was provided.
		/// </summary>
		public Func<object> ScopeCallback { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Request"/> class.
		/// </summary>
		/// <param name="service">The service that was requested.</param>
		/// <param name="constraint">The constraint that will be applied to filter the bindings used for the request.</param>
		/// <param name="parameters">The parameters that affect the resolution.</param>
		/// <param name="scopeCallback">The scope callback, if an external scope was specified.</param>
		public Request(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, Func<object> scopeCallback)
		{
			Service = service;
			Constraint = constraint;
			Parameters = parameters == null ? new List<IParameter>() : parameters.ToList();
			ActiveBindings = new List<IBinding>();
			ScopeCallback = scopeCallback;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Request"/> class.
		/// </summary>
		/// <param name="parent">The parent request.</param>
		/// <param name="service">The service that was requested.</param>
		/// <param name="target">The target that will receive the injection.</param>
		/// <param name="scopeCallback">The scope callback, if an external scope was specified.</param>
		public Request(IRequest parent, Type service, ITarget target, Func<object> scopeCallback)
		{
			Parent = parent;
			Service = service;
			Target = target;
			Constraint = target.Constraint;
			Parameters = new List<IParameter>();
			ActiveBindings = new List<IBinding>(parent.ActiveBindings);
			ScopeCallback = scopeCallback;
		}

		/// <summary>
		/// Determines whether the specified binding satisfies the constraints defined on this request.
		/// </summary>
		/// <param name="binding">The binding.</param>
		/// <returns><c>True</c> if the binding satisfies the constraints; otherwise <c>false</c>.</returns>
		public bool Matches(IBinding binding)
		{
			return Constraint == null || Constraint(binding.Metadata);
		}

		/// <summary>
		/// Gets the scope if one was specified in the request.
		/// </summary>
		/// <returns>The object that acts as the scope.</returns>
		public object GetScope()
		{
			return ScopeCallback == null ? null : ScopeCallback();
		}

		/// <summary>
		/// Creates a child request.
		/// </summary>
		/// <param name="service">The service that is being requested.</param>
		/// <param name="target">The target that will receive the injection.</param>
		/// <returns>The child request.</returns>
		public IRequest CreateChild(Type service, ITarget target)
		{
			return new Request(this, service, target, ScopeCallback);
		}
	}
}