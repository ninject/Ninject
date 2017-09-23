// -------------------------------------------------------------------------------------------------
// <copyright file="MethodInjectionDirectiveBase.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Directives
{
    using System.Linq;
    using System.Reflection;
    using Ninject.Infrastructure;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Describes the injection of a method or constructor.
    /// </summary>
    /// <typeparam name="TMethod">The method info.</typeparam>
    /// <typeparam name="TInjector">The injector.</typeparam>
    public abstract class MethodInjectionDirectiveBase<TMethod, TInjector> : IDirective
        where TMethod : MethodBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInjectionDirectiveBase{TMethod, TInjector}"/> class.
        /// </summary>
        /// <param name="method">The method this directive represents.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        protected MethodInjectionDirectiveBase(TMethod method, TInjector injector)
        {
            Ensure.ArgumentNotNull(method, "method");
            Ensure.ArgumentNotNull(injector, "injector");

            this.Injector = injector;
            this.Targets = this.CreateTargetsFromParameters(method);
        }

        /// <summary>
        /// Gets the injector that will be triggered.
        /// </summary>
        public TInjector Injector { get; private set; }

        /// <summary>
        /// Gets the targets for the directive.
        /// </summary>
        public ITarget[] Targets { get; private set; }

        /// <summary>
        /// Creates targets for the parameters of the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The targets for the method's parameters.</returns>
        protected virtual ITarget[] CreateTargetsFromParameters(TMethod method)
        {
            return method.GetParameters().Select(parameter => new ParameterTarget(method, parameter)).ToArray();
        }
    }
}