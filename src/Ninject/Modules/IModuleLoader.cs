// -------------------------------------------------------------------------------------------------
// <copyright file="IModuleLoader.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Modules
{
    using System.Collections.Generic;
    using Ninject.Components;

    /// <summary>
    /// Finds modules defined in external files.
    /// </summary>
    public interface IModuleLoader : INinjectComponent
    {
        /// <summary>
        /// Loads any modules found in the files that match the specified patterns.
        /// </summary>
        /// <param name="patterns">The patterns to search.</param>
        void LoadModules(IEnumerable<string> patterns);
    }
}