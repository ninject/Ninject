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
using Ninject.Infrastructure.Language;
#endregion

namespace Ninject.Modules
{
	/// <summary>
	/// Loads modules from compiled assemblies.
	/// </summary>
	public class CompiledModuleLoaderPlugin : NinjectComponent, IModuleLoaderPlugin
	{
		private static readonly string[] Extensions = new[] { "dll" };

		/// <summary>
		/// Gets or sets the kernel into which modules will be loaded.
		/// </summary>
		public IKernel Kernel { get; private set; }

		/// <summary>
		/// Gets the file extensions that the plugin understands how to load.
		/// </summary>
		public IEnumerable<string> SupportedExtensions
		{
			get { return Extensions; }
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
		/// <param name="filenames">The names of the files to load modules from.</param>
		public void LoadModules(IEnumerable<string> filenames)
		{
			Kernel.Load(FindAssembliesWithModules(filenames).Select(name => Assembly.Load(name)));
		}

		private static IEnumerable<AssemblyName> FindAssembliesWithModules(IEnumerable<string> filenames)
		{
			AppDomain temporaryDomain = CreateTemporaryAppDomain();

			foreach (string file in filenames)
			{
				Assembly assembly;

				try
				{
					var name = new AssemblyName { CodeBase = file };
					assembly = temporaryDomain.Load(name);
				}
				catch (BadImageFormatException)
				{
					// Ignore native assemblies
					continue;
				}

				if (assembly.HasNinjectModules())
					yield return assembly.GetName();
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