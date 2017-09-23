// -------------------------------------------------------------------------------------------------
// <copyright file="IConstructorArgumentSyntax.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Syntax
{
    using Ninject.Activation;

    /// <summary>
    /// Passed to ToConstructor to specify that a constructor value is Injected.
    /// </summary>
    public interface IConstructorArgumentSyntax : IFluentSyntax
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        IContext Context { get; }

        /// <summary>
        /// Specifies that the argument is injected.
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <returns>Not used. This interface has no implementation.</returns>
        T Inject<T>();
    }
}