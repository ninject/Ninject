// -------------------------------------------------------------------------------------------------
// <copyright file="ModuleLoader.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Modules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Ninject.Components;
    using Ninject.Infrastructure;

    /// <summary>
    /// Automatically finds and loads modules from assemblies.
    /// </summary>
    public class ModuleLoader : NinjectComponent, IModuleLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleLoader"/> class.
        /// </summary>
        /// <param name="kernel">The kernel into which modules will be loaded.</param>
        public ModuleLoader(IKernel kernel)
        {
            Ensure.ArgumentNotNull(kernel, "kernel");

            this.Kernel = kernel;
        }

        /// <summary>
        /// Gets the kernel into which modules will be loaded.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// Loads any modules found in the files that match the specified patterns.
        /// </summary>
        /// <param name="patterns">The patterns to search.</param>
        public void LoadModules(IEnumerable<string> patterns)
        {
            var plugins = this.Kernel.Components.GetAll<IModuleLoaderPlugin>();

            var fileGroups = patterns
                .SelectMany(pattern => GetFilesMatchingPattern(pattern))
                .GroupBy(filename => Path.GetExtension(filename).ToLowerInvariant());

            foreach (var fileGroup in fileGroups)
            {
                var extension = fileGroup.Key;
                var plugin = plugins.Where(p => p.SupportedExtensions.Contains(extension)).FirstOrDefault();

                if (plugin != null)
                {
                    plugin.LoadModules(fileGroup);
                }
            }
        }

        private static IEnumerable<string> GetFilesMatchingPattern(string pattern)
        {
            return NormalizePaths(Path.GetDirectoryName(pattern))
                    .SelectMany(path => Directory.GetFiles(path, Path.GetFileName(pattern)));
        }

        private static IEnumerable<string> NormalizePaths(string path)
        {
            return Path.IsPathRooted(path)
                        ? new[] { Path.GetFullPath(path) }
                        : GetBaseDirectories().Select(baseDirectory => Path.Combine(baseDirectory, path));
        }

        private static IEnumerable<string> GetBaseDirectories()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var searchPath = AppDomain.CurrentDomain.RelativeSearchPath;

            return string.IsNullOrEmpty(searchPath)
                ? new[] { baseDirectory }
                : searchPath.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => Path.Combine(baseDirectory, path));
        }
    }
}