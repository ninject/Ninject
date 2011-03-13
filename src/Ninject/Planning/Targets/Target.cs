#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.Linq;
using System.Reflection;
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets.Strategies;
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
        private readonly Future<Func<IBindingMetadata, bool>> _constraint;
        private readonly Future<bool> _isOptional;

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
            get { return _constraint; }
        }

        /// <summary>
        /// Gets a value indicating whether the target represents an optional dependency.
        /// </summary>
        public bool IsOptional
        {
            get { return _isOptional; }
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
        /// Initializes a new instance of the Target&lt;T&gt; class.
        /// </summary>
        /// <param name="member">The member that contains the target.</param>
        /// <param name="site">The site represented by the target.</param>
        protected Target(MemberInfo member, T site)
        {
            Ensure.ArgumentNotNull(member, "member");
            Ensure.ArgumentNotNull(site, "site");

            Member = member;
            Site = site;

            _constraint = new Future<Func<IBindingMetadata, bool>>(ReadConstraintFromTarget);
            _isOptional = new Future<bool>(ReadOptionalFromTarget);
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
            return Site.GetCustomAttributesExtended(attributeType, inherit);
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
            Ensure.ArgumentNotNull(attributeType, "attributeType");
            return Site.IsDefined(attributeType, inherit);
        }

        /// <summary>
        /// Resolves a value for the target within the specified parent context.
        /// </summary>
        /// <param name="parent">The parent context.</param>
        /// <returns>The resolved value.</returns>
        public object ResolveWithin(IContext parent)
        {
            Ensure.ArgumentNotNull(parent, "parent");

            var strategies = parent.Kernel.Components.GetAll<ITargetResolutionStrategy>();
            var result = strategies.Select(s => s.Resolve(this, parent))
                                   .FirstOrNone();

            if(result.HasValue)
            {
                return result.Value;
            }

            if(this.IsOptional)
            {
                return null;
            }
            
            throw new ActivationException(ExceptionFormatter.CouldNotResolveTarget(parent.Request.CreateChild(this.Type, parent, this)));
        }

        /// <summary>
        /// Reads whether the target represents an optional dependency.
        /// </summary>
        /// <returns><see langword="True"/> if it is optional; otherwise <see langword="false"/>.</returns>
        protected virtual bool ReadOptionalFromTarget()
        {
            return Site.HasAttribute(typeof(OptionalAttribute));
        }

        /// <summary>
        /// Reads the resolution constraint from target.
        /// </summary>
        /// <returns>The resolution constraint.</returns>
        protected virtual Func<IBindingMetadata, bool> ReadConstraintFromTarget()
        {
            var attributes = this.GetCustomAttributes(typeof(ConstraintAttribute), true) as ConstraintAttribute[];

            if (attributes == null || attributes.Length == 0)
                return null;

            if (attributes.Length == 1)
                return attributes[0].Matches;

            return metadata => attributes.All(attribute => attribute.Matches(metadata));
        }
    }
}