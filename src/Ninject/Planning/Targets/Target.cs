// -------------------------------------------------------------------------------------------------
// <copyright file="Target.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Targets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Infrastructure.Language;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// Represents a site on a type where a value can be injected.
    /// </summary>
    /// <typeparam name="T">The type of site this represents.</typeparam>
    public abstract class Target<T> : ITarget
        where T : ICustomAttributeProvider
    {
        private readonly Lazy<Func<IBindingMetadata, bool>> constraint;
        private readonly Lazy<bool> isOptional;

        /// <summary>
        /// Initializes a new instance of the <see cref="Target{T}"/> class.
        /// </summary>
        /// <param name="member">The member that contains the target.</param>
        /// <param name="site">The site represented by the target.</param>
        protected Target(MemberInfo member, T site)
        {
            Ensure.ArgumentNotNull(member, "member");
            Ensure.ArgumentNotNull(site, "site");

            this.Member = member;
            this.Site = site;

            this.constraint = new Lazy<Func<IBindingMetadata, bool>>(this.ReadConstraintFromTarget);
            this.isOptional = new Lazy<bool>(this.ReadOptionalFromTarget);
        }

        /// <summary>
        /// Gets the member that contains the target.
        /// </summary>
        public MemberInfo Member { get; private set; }

        /// <summary>
        /// Gets the site (property, parameter, etc.) represented by the target.
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
            get { return this.constraint.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether the target represents an optional dependency.
        /// </summary>
        public bool IsOptional
        {
            get { return this.isOptional.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether the target has a default value.
        /// </summary>
        public virtual bool HasDefaultValue
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the default value for the target.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">If the item does not have a default value.</exception>
        public virtual object DefaultValue
        {
            get { throw new InvalidOperationException(ExceptionFormatter.TargetDoesNotHaveADefaultValue(this)); }
        }

        /// <summary>
        /// Returns an array of custom attributes of a specified type defined on the target.
        /// </summary>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit">Whether to look up the hierarchy chain for inherited custom attributes.</param>
        /// <returns>An array of custom attributes of the specified type.</returns>
        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Ensure.ArgumentNotNull(attributeType, "attributeType");

            return this.Site.GetCustomAttributesExtended(attributeType, inherit);
        }

        /// <summary>
        /// Returns an array of custom attributes defined on the target.
        /// </summary>
        /// <param name="inherit">Whether to look up the hierarchy chain for inherited custom attributes.</param>
        /// <returns>An array of custom attributes.</returns>
        public object[] GetCustomAttributes(bool inherit)
        {
            return this.Site.GetCustomAttributes(inherit);
        }

        /// <summary>
        /// Returns a value indicating whether an attribute of the specified type is defined on the target.
        /// </summary>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit">Whether to look up the hierarchy chain for inherited custom attributes.</param>
        /// <returns><c>True</c> if such an attribute is defined; otherwise <c>false</c>.</returns>
        public bool IsDefined(Type attributeType, bool inherit)
        {
            Ensure.ArgumentNotNull(attributeType, "attributeType");

            return this.Site.IsDefined(attributeType, inherit);
        }

        /// <summary>
        /// Resolves a value for the target within the specified parent context.
        /// </summary>
        /// <param name="parent">The parent context.</param>
        /// <returns>The resolved value.</returns>
        public object ResolveWithin(IContext parent)
        {
            Ensure.ArgumentNotNull(parent, "parent");

            var request = parent.Request.CreateChild(this.Type, parent, this);
            request.IsUnique = true;
            return parent.Kernel.Resolve(request).SingleOrDefault();
        }

        /// <summary>
        /// Gets the value(s) that should be injected into the target.
        /// </summary>
        /// <param name="service">The service that the target is requesting.</param>
        /// <param name="parent">The parent context in which the target is being injected.</param>
        /// <returns>A series of values that are available for injection.</returns>
        [Obsolete]
        protected virtual IEnumerable<object> GetValues(Type service, IContext parent)
        {
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(parent, "parent");

            var request = parent.Request.CreateChild(service, parent, this);
            request.IsOptional = true;
            return parent.Kernel.Resolve(request);
        }

        /// <summary>
        /// Gets the value that should be injected into the target.
        /// </summary>
        /// <param name="service">The service that the target is requesting.</param>
        /// <param name="parent">The parent context in which the target is being injected.</param>
        /// <returns>The value that is to be injected.</returns>
        [Obsolete]
        protected virtual object GetValue(Type service, IContext parent)
        {
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(parent, "parent");

            var request = parent.Request.CreateChild(service, parent, this);
            request.IsUnique = true;
            return parent.Kernel.Resolve(request).SingleOrDefault();
        }

        /// <summary>
        /// Reads whether the target represents an optional dependency.
        /// </summary>
        /// <returns><see langword="True"/> if it is optional; otherwise <see langword="false"/>.</returns>
        protected virtual bool ReadOptionalFromTarget()
        {
            return this.Site.HasAttribute(typeof(OptionalAttribute));
        }

        /// <summary>
        /// Reads the resolution constraint from target.
        /// </summary>
        /// <returns>The resolution constraint.</returns>
        protected virtual Func<IBindingMetadata, bool> ReadConstraintFromTarget()
        {
            var attributes = this.GetCustomAttributes(typeof(ConstraintAttribute), true) as ConstraintAttribute[];

            if (attributes == null || attributes.Length == 0)
            {
                return null;
            }

            if (attributes.Length == 1)
            {
                return attributes[0].Matches;
            }

            return metadata => attributes.All(attribute => attribute.Matches(metadata));
        }
    }
}