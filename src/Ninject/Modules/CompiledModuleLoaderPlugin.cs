#if !SILVERLIGHT
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure;
#endregion

namespace Ninject.Modules
{
	/// <summary>
	/// Loads modules from compiled assemblies.
	/// </summary>
	public class CompiledModuleLoaderPlugin : NinjectComponent, IModuleLoaderPlugin
	{
		private static readonly string[] Patterns = new[] { "*.dll" };

		/// <summary>
		/// Gets or sets the kernel into which modules will be loaded.
		/// </summary>
		public IKernel Kernel { get; private set; }

		/// <summary>
		/// Gets the file patterns (*.dll, etc.) supported by the plugin.
		/// </summary>
		public ICollection<string> SupportedPatterns
		{
			get { return Patterns; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompiledModuleLoaderPlugin"/> class.
		/// </summary>
		/// <param name="kernel">The kernel into which modules will be loaded.</param>
		public CompiledModuleLoaderPlugin(IKernel kernel)
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			Kernel = kernel;
		}

		/// <summary>
		/// Loads modules from the specified files.
		/// </summary>
		/// <param name="files">The names of the files to load modules from.</param>
		public void LoadModules(IEnumerable<string> files)
		{
			foreach (AssemblyName assemblyName in FindAssembliesWithModules(files))
				LoadModulesFromAssembly(Assembly.Load(assemblyName));
		}

		/// <summary>
		/// Loads any modules defined in the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly to search for modules.</param>
		protected virtual void LoadModulesFromAssembly(Assembly assembly)
		{
			foreach (Type type in assembly.GetExportedTypes().Where(IsLoadableModule))
			{
				var module = Activator.CreateInstance(type) as IModule;

				if (Kernel.HasModule(module.Name))
					continue;

				Kernel.LoadModule(module);
			}
		}

		/// <summary>
		/// Determines whether the specified type is a loadable module.
		/// </summary>
		/// <param name="type">The type in question.</param>
		/// <returns><see langword="True"/> if the type represents a loadable module; otherwise <see langword="false"/>.</returns>
		protected virtual bool IsLoadableModule(Type type)
		{
			Ensure.ArgumentNotNull(type, "type");

			if (!typeof(IModule).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
				return false;

			return type.GetConstructor(Type.EmptyTypes) != null;
		}

		private IEnumerable<AssemblyName> FindAssembliesWithModules(IEnumerable<string> files)
		{
			AppDomain temporaryDomain = CreateTemporaryAppDomain();

			foreach (string file in files)
			{
				var assemblyName = new AssemblyName { CodeBase = file };

				Assembly assembly;

				try
				{
					assembly = temporaryDomain.Load(assemblyName);
				}
				catch (BadImageFormatException)
				{
					// Ignore native assemblies
					continue;
				}

				if (assembly.GetExportedTypes().Any(IsLoadableModule))
					yield return assemblyName;
			}

			AppDomain.Unload(temporaryDomain);
		}

		private static AppDomain CreateTemporaryAppDomain()
		{
			return AppDomain.CreateDomain(
				"NinjectModuleLoader",
				AppDomain.CurrentDomain.Evidence,
				AppDomain.CurrentDomain.BaseDirectory,
				AppDomain.CurrentDomain.RelativeSearchPath,
				false);
		}
	}
}
#endif //!SILVERLIGHT