// -------------------------------------------------------------------------------------------------
// <copyright file="IModuleLoaderPlugin.cs" company="Ninject Project Contributors">
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
    /// Loads modules at runtime by searching external files.
    /// </summary>
    public interface IModuleLoaderPlugin : INinjectComponent
    {
        /// <summary>
        /// Gets the file extensions that the plugin understands how to load.
        /// </summary>
        IEnumerable<string> SupportedExtensions { get; }

        /// <summary>
        /// Loads modules from the specified files.
        /// </summary>
        /// <param name="filenames">The names of the files to load modules from.</param>
        void LoadModules(IEnumerable<string> filenames);
    }
}