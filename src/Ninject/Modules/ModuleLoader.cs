#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion

#if !NO_ASSEMBLY_SCANNING
#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ninject.Components;
using Ninject.Infrastructure;
#endregion

namespace Ninject.Modules
{
    /// <summary>
    /// Automatically finds and loads modules from assemblies.
    /// </summary>
    public class ModuleLoader : NinjectComponent, IModuleLoader
    {
        /// <summary>
        /// Gets or sets the kernel into which modules will be loaded.
        /// </summary>
        public IKernelConfiguration KernelConfiguration { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleLoader"/> class.
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration into which modules will be loaded.</param>
        public ModuleLoader(IKernelConfiguration kernelConfiguration)
        {
            KernelConfiguration = kernelConfiguration;
        }
#if !PCL
        /// <summary>
        /// Loads any modules found in the files that match the specified patterns.
        /// </summary>
        /// <param name="patterns">The patterns to search.</param>
        public
#if !WINRT
 void
#else
 async System.Threading.Tasks.Task
#endif
            LoadModules(IEnumerable<string> patterns)
        {
#if PCL
            throw new NotImplementedException();
#else
            var plugins = KernelConfiguration.Components.GetAll<IModuleLoaderPlugin>();

            var fileGroups = patterns
#if !WINRT
                .SelectMany(pattern => GetFilesMatchingPattern(pattern))
                .GroupBy(filename => Path.GetExtension(filename).ToLowerInvariant());
#else
                .GroupBy(filename => GetExtension(filename).ToLowerInvariant());
#endif          

            foreach (var fileGroup in fileGroups)
            {
                string extension = fileGroup.Key;
                IModuleLoaderPlugin plugin = plugins.Where(p => p.SupportedExtensions.Contains(extension)).FirstOrDefault();

                if (plugin != null)
#if WINRT
                    await 
#endif               
                    plugin.LoadModules(fileGroup);
            }
#endif
        }
#endif

#if !PCL
#if WINRT
        private static string GetExtension(string filename)
        {
            var i = filename.LastIndexOf('.');
            return filename.Substring(i);
        }
#endif
#if !WINRT
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
#if ANDROID
            return new[] { Android.App.Application.Context.FilesDir.AbsolutePath };
#else
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var searchPath = AppDomain.CurrentDomain.RelativeSearchPath;

            return String.IsNullOrEmpty(searchPath) 
                ? new[] {baseDirectory} 
                : searchPath.Split(new[] {Path.PathSeparator}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(path => Path.Combine(baseDirectory, path));
#endif
        }
#endif
#endif
    }
}
#endif //!NO_ASSEMBLY_SCANNING