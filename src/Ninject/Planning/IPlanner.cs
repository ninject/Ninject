// -------------------------------------------------------------------------------------------------
// <copyright file="IPlanner.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning
{
    using System;
    using System.Collections.Generic;
    using Ninject.Components;
    using Ninject.Planning.Strategies;

    /// <summary>
    /// Generates plans for how to activate instances.
    /// </summary>
    public interface IPlanner : INinjectComponent
    {
        /// <summary>
        /// Gets the strategies that contribute to the planning process.
        /// </summary>
        IList<IPlanningStrategy> Strategies { get; }

        /// <summary>
        /// Gets or creates an activation plan for the specified type.
        /// </summary>
        /// <param name="type">The type for which a plan should be created.</param>
        /// <returns>The type's activation plan.</returns>
        IPlan GetPlan(Type type);
    }
}