// -------------------------------------------------------------------------------------------------
// <copyright file="IPlanningStrategy.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Strategies
{
    using Ninject.Components;

    /// <summary>
    /// Contributes to the generation of a <see cref="IPlan"/>.
    /// </summary>
    public interface IPlanningStrategy : INinjectComponent
    {
        /// <summary>
        /// Contributes to the specified plan.
        /// </summary>
        /// <param name="plan">The plan that is being generated.</param>
        void Execute(IPlan plan);
    }
}