#region License
// Author: Nate Kohari <nkohari@gmail.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Reflection;
using Ninject.Modules;
#endregion

namespace Ninject
{
	/// <summary>
	/// Extension methods that enhance module loading.
	/// </summary>
	public static class ModuleLoadExtensions
	{
		private static readonly string[] DefaultPatterns = new[] { "*.dll", "*.exe" };

		/// <summary>
		/// Determines whether a module of the specified type has been loaded in the kernel.
		/// </summary>
		/// <typeparam name="TModule">The type of the module.</typeparam>
		/// <returns><c>True</c> if the specified module has been loaded; otherwise, <c>false</c>.</returns>
		public static bool HasModule<TModule>(this IKernel kernel)
			where TModule : IModule
		{
			return kernel.HasModule(typeof(TModule));
		}

		/// <summary>
		/// Creates a new instance of the module and loads it into the kernel.
		/// </summary>
		/// <typeparam name="TModule">The type of the module.</typeparam>
		public static void LoadModule<TModule>(this IKernel kernel)
			where TModule : IModule, new()
		{
			kernel.LoadModule(new TModule());
		}

		/// <summary>
		/// Unloads the module with the specified type.
		/// </summary>
		/// <typeparam name="TModule">The type of the module.</typeparam>
		public static void UnloadModule<TModule>(this IKernel kernel)
			where TModule : IModule
		{
			kernel.UnloadModule(typeof(TModule));
		}

		/// <summary>
		/// Scans the application's base directory for assemblies, and if they have loadable modules, loads them.
		/// </summary>
		public static void AutoLoadModules(this IKernel kernel)
		{
			GetModuleLoader(kernel).ScanAndLoadModules("~", DefaultPatterns, false);
		}

		/// <summary>
		/// Scans the specified path for assemblies, and if they have loadable modules, loads them.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		/// <param name="path">The path to search.</param>
		public static void AutoLoadModules(this IKernel kernel, string path)
		{
			GetModuleLoader(kernel).ScanAndLoadModules(path, DefaultPatterns, false);
		}

		/// <summary>
		/// Scans the specified path for assemblies that match the specified search pattern(s), and
		/// if they have loadable modules, loads them.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		/// <param name="path">The path to search.</param>
		/// <param name="patterns">The file search patterns to match.</param>
		public static void AutoLoadModules(this IKernel kernel, string path, params string[] patterns)
		{
			GetModuleLoader(kernel).ScanAndLoadModules(path, patterns, false);
		}

		/// <summary>
		/// Scans the application's base directory and all subdirectories for assemblies, and if
		/// they have loadable modules, loads them.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		public static void AutoLoadModulesRecursively(this IKernel kernel)
		{
			GetModuleLoader(kernel).ScanAndLoadModules("~", DefaultPatterns, true);
		}

		/// <summary>
		/// Scans the specified path and all subdirectories for assemblies, and if they have
		/// loadable modules, loads them.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		/// <param name="path">The path to search.</param>
		public static void AutoLoadModulesRecursively(this IKernel kernel, string path)
		{
			GetModuleLoader(kernel).ScanAndLoadModules(path, DefaultPatterns, true);
		}

		/// <summary>
		/// Scans the specified path and all subdirectories for assemblies that match the specified
		/// search pattern(s), and if they have loadable modules, loads them.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		/// <param name="path">The path to search.</param>
		/// <param name="patterns">The file search patterns to match.</param>
		public static void AutoLoadModulesRecursively(this IKernel kernel, string path, params string[] patterns)
		{
			GetModuleLoader(kernel).ScanAndLoadModules(path, patterns, true);
		}

		/// <summary>
		/// Loads any loadable modules from the assembly.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		/// <param name="assembly">The assembly to scan for modules.</param>
		public static void LoadModulesFromAssembly(this IKernel kernel, Assembly assembly)
		{
			GetModuleLoader(kernel).LoadModules(assembly);
		}

		/// <summary>
		/// Loads any loadable modules from the assembly.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		/// <param name="assemblyOrFileName">The assembly name or filename of the assembly to load modules from.</param>
		public static void LoadModulesFromAssembly(this IKernel kernel, string assemblyOrFileName)
		{
			GetModuleLoader(kernel).LoadModules(assemblyOrFileName);
		}

		private static IModuleLoader GetModuleLoader(IKernel kernel)
		{
			return kernel.Components.Get<IModuleLoader>();
		}
	}
}
