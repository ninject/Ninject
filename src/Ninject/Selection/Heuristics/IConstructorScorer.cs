// -------------------------------------------------------------------------------------------------
// <copyright file="IConstructorScorer.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Selection.Heuristics
{
    using Ninject.Activation;
    using Ninject.Components;
    using Ninject.Planning.Directives;

    /// <summary>
    /// Generates scores for constructors, to determine which is the best one to call during activation.
    /// </summary>
    public interface IConstructorScorer : INinjectComponent
    {
        /// <summary>
        /// Gets the score for the specified constructor.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        int Score(IContext context, ConstructorInjectionDirective directive);
    }
}