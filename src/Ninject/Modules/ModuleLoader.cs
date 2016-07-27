//-------------------------------------------------------------------------------------------------
// <copyright file="ModuleLoader.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------------------------
#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Modules
{
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Ninject.Components;

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
            Contract.Requires(kernel != null);
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
                var plugin = plugins.FirstOrDefault(p => p.SupportedExtensions.Contains(extension));

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
#endif //!NO_ASSEMBLY_SCANNING