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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Activation;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Planning.Targets
{
    /// <summary>
    /// Represents a site on a type where a value can be injected.
    /// </summary>
    /// <typeparam name="T">The type of site this represents.</typeparam>
#if !WINRT 
    public abstract class Target<T> : ITarget
#if !PCL
        where T : ICustomAttributeProvider
#endif
#else
    public abstract class Target : ITarget
#endif
    {
        private readonly Future<Func<IBindingMetadata, bool>> constraint;
        private readonly Future<bool> isOptional;

        /// <summary>
        /// Gets the member that contains the target.
        /// </summary>
        public MemberInfo Member { get; private set; }

#if !WINRT
        /// <summary>
        /// Gets or sets the site (property, parameter, etc.) represented by the target.
        /// </summary>
        public T Site { get; private set; }
#endif
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
            get { return this.constraint; }
        }

        /// <summary>
        /// Gets a value indicating whether the target represents an optional dependency.
        /// </summary>
        public bool IsOptional
        {
            get { return this.isOptional; }
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

#if !WINRT
        /// <summary>
        /// Initializes a new instance of the Target&lt;T&gt; class.
        /// </summary>
        /// <param name="member">The member that contains the target.</param>
        /// <param name="site">The site represented by the target.</param>
        protected Target(MemberInfo member, T site)
        {
            this.Member = member;
            this.Site = site;

            this.constraint = new Future<Func<IBindingMetadata, bool>>(ReadConstraintFromTarget);
            this.isOptional = new Future<bool>(ReadOptionalFromTarget);
        }

#else
        /// <summary>
        /// Initializes a new instance of the Target&lt;T&gt; class.
        /// </summary>
        /// <param name="member">The member that contains the target.</param>
        /// <param name="site">The site represented by the target.</param>
        protected Target(MemberInfo member)
        {
            Ensure.ArgumentNotNull(member, "member");

            Member = member;

            constraint = new Future<Func<IBindingMetadata, bool>>(ReadConstraintFromTarget);
            isOptional = new Future<bool>(ReadOptionalFromTarget);
        }
#endif

        /// <summary>
        /// Returns an array of custom attributes of a specified type defined on the target.
        /// </summary>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit">Whether to look up the hierarchy chain for inherited custom attributes.</param>
        /// <returns>An array of custom attributes of the specified type.</returns>
        public
#if WINRT
            abstract
#endif
            IEnumerable<Attribute>

            GetCustomAttributes(Type attributeType, bool inherit)
#if WINRT
            ;
#else
        {
#if PCL
            throw new NotImplementedException();
#else
            return Site.GetCustomAttributesExtended(attributeType, inherit);
#endif
        }
#endif

        /// <summary>
        /// Returns an array of custom attributes defined on the target.
        /// </summary>
        /// <param name="inherit">Whether to look up the hierarchy chain for inherited custom attributes.</param>
        /// <returns>An array of custom attributes.</returns>
        public
#if WINRT
 abstract 
#endif
            IEnumerable<Attribute>

            GetCustomAttributes(bool inherit)
#if WINRT
            ;
#else
        {
            #if PCL
            throw new NotImplementedException();
#else
            return Site.GetCustomAttributes(inherit).Cast<Attribute>();
#endif
        }
#endif

        /// <summary>
        /// Returns a value indicating whether an attribute of the specified type is defined on the target.
        /// </summary>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit">Whether to look up the hierarchy chain for inherited custom attributes.</param>
        /// <returns><c>True</c> if such an attribute is defined; otherwise <c>false</c>.</returns>
        public
#if WINRT
            abstract
#endif
            bool IsDefined(Type attributeType, bool inherit)
#if WINRT
            ;
#else
        {
#if PCL
            throw new NotImplementedException();
#else
            return Site.IsDefined(attributeType, inherit);
#endif
        }
#endif

        /// <summary>
        /// Determines whether the parent has attribute.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>
        /// 	<c>true</c> if the specified member has attribute; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDefinedOnParent(Type attributeType, Type parent)
        {
            return parent.GetTypeInfo().HasAttribute(attributeType);
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
                return GetValues(service, parent).CastSlow(service).ToArraySlow(service);
            }

            if (Type.GetTypeInfo().IsGenericType)
            {
                Type gtd = Type.GetGenericTypeDefinition();

                Type service = Type.GenericTypeArguments[0];

                if (gtd == typeof(List<>) || gtd == typeof(IList<>) || gtd == typeof(ICollection<>))
                    return GetValues(service, parent).CastSlow(service).ToListSlow(service);

                if (gtd == typeof(IEnumerable<>))
                    return GetValues(service, parent).CastSlow(service);
            }

            return GetValue(Type, parent);
        }

        /// <summary>
        /// Gets the value(s) that should be injected into the target.
        /// </summary>
        /// <param name="service">The service that the target is requesting.</param>
        /// <param name="parent">The parent context in which the target is being injected.</param>
        /// <returns>A series of values that are available for injection.</returns>
        protected virtual IEnumerable<object> GetValues(Type service, IContext parent)
        {
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
        protected virtual object GetValue(Type service, IContext parent)
        {
            var request = parent.Request.CreateChild(service, parent, this);
            request.IsUnique = true;
            return parent.Kernel.Resolve(request).SingleOrDefault();
        }

        /// <summary>
        /// Reads whether the target represents an optional dependency.
        /// </summary>
        /// <returns><see langword="True"/> if it is optional; otherwise <see langword="false"/>.</returns>
        protected
#if !WINRT
            virtual
#else
            abstract
#endif
            bool ReadOptionalFromTarget()
#if WINRT
            ;
#else
        {
            #if PCL
            throw new NotImplementedException();
#else
            return Site.HasAttribute(typeof(OptionalAttribute));
#endif
        }
#endif

        /// <summary>
        /// Reads the resolution constraint from target.
        /// </summary>
        /// <returns>The resolution constraint.</returns>
        protected virtual Func<IBindingMetadata, bool> ReadConstraintFromTarget()
        {
            var attributes = this.GetCustomAttributes(typeof(ConstraintAttribute), true).Cast<ConstraintAttribute>().ToList();

            if (attributes == null || attributes.Count == 0)
                return null;

            if (attributes.Count == 1)
                return attributes[0].Matches;

            return metadata => attributes.All(attribute => attribute.Matches(metadata));
        }
    }
}