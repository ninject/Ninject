// -------------------------------------------------------------------------------------------------
// <copyright file="MethodInjectionDirective.cs" company="Ninject Project Contributors">
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
    /// Describes the injection of a method.
    /// </summary>
    public class MethodInjectionDirective : MethodInjectionDirectiveBase<MethodInfo, MethodInjector>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInjectionDirective"/> class.
        /// </summary>
        /// <param name="method">The method described by the directive.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public MethodInjectionDirective(MethodInfo method, MethodInjector injector)
            : base(method, injector)
        {
        }
    }
}