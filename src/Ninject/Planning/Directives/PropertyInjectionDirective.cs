// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyInjectionDirective.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Directives
{
    using System.Reflection;
    using Ninject.Injection;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Describes the injection of a property.
    /// </summary>
    public class PropertyInjectionDirective : IDirective
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInjectionDirective"/> class.
        /// </summary>
        /// <param name="member">The member the directive describes.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public PropertyInjectionDirective(PropertyInfo member, PropertyInjector injector)
        {
            this.Injector = injector;
            this.Target = this.CreateTarget(member);
        }

        /// <summary>
        /// Gets the injector that will be triggered.
        /// </summary>
        public PropertyInjector Injector { get; private set; }

        /// <summary>
        /// Gets the injection target for the directive.
        /// </summary>
        public ITarget Target { get; private set; }

        /// <summary>
        /// Creates a target for the property.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        /// <returns>The target for the property.</returns>
        protected virtual ITarget CreateTarget(PropertyInfo propertyInfo)
        {
            return new PropertyTarget(propertyInfo);
        }
    }
}