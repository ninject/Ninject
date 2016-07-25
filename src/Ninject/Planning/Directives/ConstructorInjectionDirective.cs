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
using System.Reflection;
using Ninject.Infrastructure;
using Ninject.Injection;
#endregion

namespace Ninject.Planning.Directives
{
    /// <summary>
    /// Describes the injection of a constructor.
    /// </summary>
    public class ConstructorInjectionDirective : MethodInjectionDirectiveBase<ConstructorInfo, ConstructorInjector>
    {
        /// <summary>
        /// The base .ctor definition.
        /// </summary>
        public ConstructorInfo Constructor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this constructor has an inject attribute.
        /// </summary>
        /// <value><c>true</c> if this constructor has an inject attribute; otherwise, <c>false</c>.</value>
        public bool HasInjectAttribute { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorInjectionDirective"/> class.
        /// </summary>
        /// <param name="service">The service this directive represents.</param>
        /// <param name="constructor">The constructor described by the directive.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public ConstructorInjectionDirective(Type service, ConstructorInfo constructor, ConstructorInjector injector)
            : base(service, constructor, injector)
        {
            Constructor = constructor;
        }
    }
}