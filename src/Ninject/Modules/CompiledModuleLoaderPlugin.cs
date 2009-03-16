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
		/// <param name="filenames">The names of the files to load modules from.</param>
		public void LoadModules(IEnumerable<string> filenames)
		{
			foreach (Type type in AssemblyScanner.FindMatchingTypesInAssemblies(filenames, IsLoadableModule))
			{
				var module = Activator.CreateInstance(type) as INinjectModule;
				Kernel.LoadModule(module);
			}
		}

		private static bool IsLoadableModule(Type type)
		{
			return typeof(INinjectModule).IsAssignableFrom(type)
				&& !type.IsAbstract
				&& !type.IsInterface
				&& type.GetConstructor(Type.EmptyTypes) != null;
		}
	}
}
#endif //!SILVERLIGHT