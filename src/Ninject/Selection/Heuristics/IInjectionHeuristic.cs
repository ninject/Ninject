// -------------------------------------------------------------------------------------------------
// <copyright file="IInjectionHeuristic.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Selection.Heuristics
{
    using System.Reflection;
    using Ninject.Components;

    /// <summary>
    /// Determines whether members should be injected during activation.
    /// </summary>
    public interface IInjectionHeuristic : INinjectComponent
    {
        /// <summary>
        /// Returns a value indicating whether the specified member should be injected.
        /// </summary>
        /// <param name="member">The member in question.</param>
        /// <returns><c>True</c> if the member should be injected; otherwise <c>false</c>.</returns>
        bool ShouldInject(MemberInfo member);
    }
}