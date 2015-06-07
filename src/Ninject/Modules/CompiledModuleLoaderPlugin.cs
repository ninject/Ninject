//-------------------------------------------------------------------------------
// <copyright file="CompiledModuleLoaderPlugin.cs" company="Ninject Project Contributors">
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
//-------------------------------------------------------------------------------

using System;

#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ninject.Components;
    using Ninject.Infrastructure.Language;
    
    /// <summary>
    /// Loads modules from compiled assemblies.
    /// </summary>
    public class CompiledModuleLoaderPlugin : NinjectComponent, IModuleLoaderPlugin
    {
        /// <summary>
        /// The assembly name retriever.
        /// </summary>
        private readonly IAssemblyNameRetriever assemblyNameRetriever;

        /// <summary>
        /// The file extensions that are supported.
        /// </summary>
        private static readonly string[] Extensions = { ".dll" };

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledModuleLoaderPlugin"/> class.
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration into which modules will be loaded.</param>
        /// <param name="assemblyNameRetriever">The assembly name retriever.</param>
        public CompiledModuleLoaderPlugin(IKernelConfiguration kernelConfiguration, IAssemblyNameRetriever assemblyNameRetriever)
        {
            this.KernelConfiguration = kernelConfiguration;
            this.assemblyNameRetriever = assemblyNameRetriever;
        }

        /// <summary>
        /// Gets the kernel configuration into which modules will be loaded.
        /// </summary>
        public IKernelConfiguration KernelConfiguration { get; private set; }

        /// <summary>
        /// Gets the file extensions that the plugin understands how to load.
        /// </summary>
        public IEnumerable<string> SupportedExtensions
        {
            get { return Extensions; }
        }

        /// <summary>
        /// Loads modules from the specified files.
        /// </summary>
        /// <param name="filenames">The names of the files to load modules from.</param>
        public
#if !WINRT
        void 
#else
 async System.Threading.Tasks.Task
#endif
            LoadModules(IEnumerable<string> filenames)
        {
#if PCL
            throw new NotImplementedException();
#else
            var assembliesWithModules = 
#if WINRT
                await 
#endif
            this.assemblyNameRetriever.GetAssemblyNames(filenames, asm => asm.HasNinjectModules());


            this.KernelConfiguration.Load(assembliesWithModules.Select(asm => Assembly.Load(asm)));
#endif
        }
    }
}
#endif