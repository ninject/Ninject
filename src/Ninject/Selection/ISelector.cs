// -------------------------------------------------------------------------------------------------
// <copyright file="ISelector.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Selection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Ninject.Components;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Selects members for injection.
    /// </summary>
    public interface ISelector : INinjectComponent
    {
        /// <summary>
        /// Gets the constructor scorer.
        /// </summary>
        IConstructorScorer ConstructorScorer { get; }

        /// <summary>
        /// Gets the heuristics used to determine which members should be injected.
        /// </summary>
        ICollection<IInjectionHeuristic> InjectionHeuristics { get; }

        /// <summary>
        /// Selects the constructor to call on the specified type, by using the constructor scorer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The selected constructor, or <see langword="null"/> if none were available.</returns>
        IEnumerable<ConstructorInfo> SelectConstructorsForInjection(Type type);

        /// <summary>
        /// Selects properties that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected properties.</returns>
        IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type);

        /// <summary>
        /// Selects methods that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected methods.</returns>
        IEnumerable<MethodInfo> SelectMethodsForInjection(Type type);
    }
}