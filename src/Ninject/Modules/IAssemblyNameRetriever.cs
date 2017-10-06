// -------------------------------------------------------------------------------------------------
// <copyright file="IAssemblyNameRetriever.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Ninject.Components;

    /// <summary>
    /// Retrieves assembly names from file names using a temporary app domain.
    /// </summary>
    public interface IAssemblyNameRetriever : INinjectComponent
    {
        /// <summary>
        /// Gets all assembly names of the assemblies in the given files that match the filter.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>All assembly names of the assemblies in the given files that match the filter.</returns>
        IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames, Predicate<Assembly> filter);
    }
}