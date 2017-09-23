// -------------------------------------------------------------------------------------------------
// <copyright file="ConstructorInjectionDirective.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Directives
{
    using System.Reflection;
    using Ninject.Injection;

    /// <summary>
    /// Describes the injection of a constructor.
    /// </summary>
    public class ConstructorInjectionDirective : MethodInjectionDirectiveBase<ConstructorInfo, ConstructorInjector>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorInjectionDirective"/> class.
        /// </summary>
        /// <param name="constructor">The constructor described by the directive.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public ConstructorInjectionDirective(ConstructorInfo constructor, ConstructorInjector injector)
            : base(constructor, injector)
        {
            this.Constructor = constructor;
        }

        /// <summary>
        /// Gets or sets the base .ctor definition.
        /// </summary>
        public ConstructorInfo Constructor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this constructor has an inject attribute.
        /// </summary>
        /// <value><c>true</c> if this constructor has an inject attribute; otherwise, <c>false</c>.</value>
        public bool HasInjectAttribute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this contructor has an obsolete attribute.
        /// </summary>
        /// <value><c>true</c> if this constructor has an obsolete attribute; otherwise, <c>false</c>.</value>
        public bool HasObsoleteAttribute { get; set; }
    }
}