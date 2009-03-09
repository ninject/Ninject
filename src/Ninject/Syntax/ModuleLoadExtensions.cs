#region License
// Author: Nate Kohari <nate@enkari.com>
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
using Ninject.Infrastructure;
using Ninject.Modules;
#endregion

namespace Ninject
{
	/// <summary>
	/// Extension methods that enhance module loading.
	/// </summary>
	public static class ModuleLoadExtensions
	{
		/// <summary>
		/// Determines whether a module of the specified type has been loaded in the kernel.
		/// </summary>
		/// <typeparam name="TModule">The type of the module.</typeparam>
		/// <returns><c>True</c> if the specified module has been loaded; otherwise, <c>false</c>.</returns>
		public static bool HasModule<TModule>(this IKernel kernel)
			where TModule : IModule
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			return kernel.HasModule(typeof(TModule));
		}

		/// <summary>
		/// Creates a new instance of the module and loads it into the kernel.
		/// </summary>
		/// <typeparam name="TModule">The type of the module.</typeparam>
		public static void LoadModule<TModule>(this IKernel kernel)
			where TModule : IModule, new()
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			kernel.LoadModule(new TModule());
		}

		/// <summary>
		/// Unloads the module with the specified type.
		/// </summary>
		/// <typeparam name="TModule">The type of the module.</typeparam>
		public static void UnloadModule<TModule>(this IKernel kernel)
			where TModule : IModule
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			kernel.UnloadModule(typeof(TModule));
		}

		/// <summary>
		/// Scans the application's base directory for assemblies, and if they have loadable modules, loads them.
		/// </summary>
		public static void AutoLoadModules(this IKernel kernel)
		{
			GetModuleLoader(kernel).LoadModules("~");
		}

		/// <summary>
		/// Scans the specified path for assemblies, and if they have loadable modules, loads them.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		/// <param name="path">The path to search.</param>
		public static void AutoLoadModules(this IKernel kernel, string path)
		{
			GetModuleLoader(kernel).LoadModules(path);
		}

		/// <summary>
		/// Scans the application's base directory and all subdirectories for assemblies, and if
		/// they have loadable modules, loads them.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		public static void AutoLoadModulesRecursively(this IKernel kernel)
		{
			GetModuleLoader(kernel).LoadModules("~", true);
		}

		/// <summary>
		/// Scans the specified path and all subdirectories for assemblies, and if they have
		/// loadable modules, loads them.
		/// </summary>
		/// <param name="kernel">The kernel to load the modules into.</param>
		/// <param name="path">The path to search.</param>
		public static void AutoLoadModulesRecursively(this IKernel kernel, string path)
		{
			GetModuleLoader(kernel).LoadModules(path, true);
		}

		private static IModuleLoader GetModuleLoader(IKernel kernel)
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			return kernel.Components.Get<IModuleLoader>();
		}
	}
}
