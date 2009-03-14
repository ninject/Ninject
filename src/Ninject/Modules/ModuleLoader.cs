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
using System.IO;
using System.Linq;
#if !NO_WEB
using System.Web;
#endif
using Ninject.Components;
using Ninject.Infrastructure;
#endregion

namespace Ninject.Modules
{
	/// <summary>
	/// Automatically finds and loads modules from assemblies.
	/// </summary>
	public class ModuleLoader : NinjectComponent, IDecoratableModuleLoader
	{
		/// <summary>
		/// Gets or sets the kernel into which modules will be loaded.
		/// </summary>
		public IKernel Kernel { get; private set; }

		/// <summary>
		/// Gets or sets the plugins.
		/// </summary>
		public ICollection<IModuleLoaderPlugin> Plugins { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleLoader"/> class.
		/// </summary>
		/// <param name="kernel">The kernel into which modules will be loaded.</param>
		/// <param name="plugins">The plugins that will be used to load modules from files.</param>
		public ModuleLoader(IKernel kernel, IEnumerable<IModuleLoaderPlugin> plugins)
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			Ensure.ArgumentNotNull(plugins, "plugins");

			Kernel = kernel;
			Plugins = plugins.ToList();
		}

		/// <summary>
		/// Loads any modules found in files in the specified path.
		/// </summary>
		/// <param name="path">The path to search.</param>
		public void LoadModules(string path)
		{
			LoadModules(path, false);
		}

		/// <summary>
		/// Loads any modules found in files in the specified path.
		/// </summary>
		/// <param name="path">The path to search.</param>
		/// <param name="recursive">If <see langword="true"/>, search the path's subdirectories as well.</param>
		public void LoadModules(string path, bool recursive)
		{
			Ensure.ArgumentNotNull(path, "path");

			foreach (IModuleLoaderPlugin plugin in Plugins)
			{
				var files = FindFiles(path, plugin.SupportedPatterns, recursive);
				plugin.LoadModules(files);
			}
		}

		/// <summary>
		/// Finds files in the specified path, matching the specified patterns.
		/// </summary>
		/// <param name="path">The path to search.</param>
		/// <param name="patterns">The patterns to match.</param>
		/// <param name="recursive">If <see langword="true"/>, search the path's subdirectories as well.</param>
		/// <returns>A series of filenames that match the criteria.</returns>
		protected virtual IEnumerable<string> FindFiles(string path, IEnumerable<string> patterns, bool recursive)
		{
			Ensure.ArgumentNotNullOrEmpty(path, "path");
			Ensure.ArgumentNotNull(patterns, "patterns");

			var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var directory = new DirectoryInfo(NormalizePath(path));

			return patterns.SelectMany(pattern => directory.GetFiles(pattern, searchOption).Select(fi => fi.FullName));
		}

		/// <summary>
		/// Normalizes the provided path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>The normalized path.</returns>
		protected virtual string NormalizePath(string path)
		{
			Ensure.ArgumentNotNullOrEmpty(path, "path");

			if (path.StartsWith("~"))
				path = GetBaseDirectory() + path.Substring(1);

			return new DirectoryInfo(path).FullName;
		}

		private static string GetBaseDirectory()
		{
			#if NO_WEB
			return AppDomain.CurrentDomain.BaseDirectory;
			#else
			return HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~") : AppDomain.CurrentDomain.BaseDirectory;
			#endif
		}
	}
}
#endif //!SILVERLIGHT