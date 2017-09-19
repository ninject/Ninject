//-------------------------------------------------------------------------------------------------
// <copyright file="ModuleLoadExtensions.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2016, Ninject Project Contributors
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

namespace Ninject
{
    using System.Reflection;
    using Ninject.Modules;

    /// <summary>
    /// Extension methods that enhance module loading.
    /// </summary>
    public static class ModuleLoadExtensions
    {
        /// <summary>
        /// Creates a new instance of the module and loads it into the kernel.
        /// </summary>
        /// <typeparam name="TModule">The type of the module.</typeparam>
        /// <param name="kernelConfiguration">The kernel configuration into which the module is loaded.</param>
        public static void Load<TModule>(this IKernelConfiguration kernelConfiguration)
            where TModule : INinjectModule, new()
        {
            kernelConfiguration.Load(new TModule());
        }

        /// <summary>
        /// Loads the module(s) into the kernel.
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration.</param>
        /// <param name="modules">The modules to load into which the modules are loaded.</param>
        public static void Load(this IKernelConfiguration kernelConfiguration, params INinjectModule[] modules)
        {
            kernelConfiguration.Load(modules);
        }

		/// <summary>
		/// Creates a new instance of the module and loads it into the kernel if it is not loaded.
		/// </summary>
		/// <typeparam name="TModule">The type of module.</typeparam>
		/// <param name="kernelConfiguration">The kernel configuration into which the module is loaded.</param>
		public static void LoadIfNotLoaded<TModule>(this IKernelConfiguration kernelConfiguration)
			where TModule : INinjectModule, new()
		{
			kernelConfiguration.LoadIfNotLoaded(new TModule());
		}

		/// <summary>
		/// Loads the module(s) into the kernel if the module is not loaded.
		/// </summary>
		/// <param name="kernelConfiguration">The kernel configuration.</param>
		/// <param name="modules">The modules to load into which the modules are loaded.</param>
		public static void LoadIfNotLoaded(this IKernelConfiguration kernelConfiguration, params INinjectModule[] modules)
		{
			foreach (var module in modules)
			{
				if (kernelConfiguration.HasModule(module.Name))
				{
					kernelConfiguration.Load(module);
				}
			}
		}

        #if !NO_ASSEMBLY_SCANNING
        /// <summary>
        /// Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration into which the files are loaded.</param>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        public static void Load(this IKernelConfiguration kernelConfiguration, params string[] filePatterns)
        {
            kernelConfiguration.Load(filePatterns);
        }

        /// <summary>
        /// Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration into which the assemblies are loaded.</param>
        /// <param name="assemblies">The assemblies to search.</param>
        public static void Load(this IKernelConfiguration kernelConfiguration, params Assembly[] assemblies)
        {
            kernelConfiguration.Load(assemblies);
        }
        #endif
    }
}