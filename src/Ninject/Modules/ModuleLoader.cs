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
using System.Text.RegularExpressions;
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
	public class ModuleLoader : NinjectComponent, IModuleLoader
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
		/// Loads any modules found in the files that match the specified patterns.
		/// </summary>
		/// <param name="patterns">The patterns to search.</param>
		public void LoadModules(IEnumerable<string> patterns)
		{
			IEnumerable<string> matchingFiles = patterns.SelectMany(p => GetFilesMatchingPattern(p));

			foreach (IModuleLoaderPlugin plugin in Plugins)
				plugin.LoadModules(matchingFiles);
		}

		private static string[] GetFilesMatchingPattern(string pattern)
		{
			string path = NormalizePath(Path.GetDirectoryName(pattern));
			string glob = Path.GetFileName(pattern);

			return Directory.GetFiles(path, glob);
		}

		private static string NormalizePath(string path)
		{
			if (!Path.IsPathRooted(path))
				path = Path.Combine(GetSearchPath(), path);

			return Path.GetFullPath(path);
		}

		private static string GetSearchPath()
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string searchPath = AppDomain.CurrentDomain.RelativeSearchPath;

			return String.IsNullOrEmpty(searchPath) ? baseDirectory : Path.Combine(baseDirectory, searchPath);
		}
	}
}
#endif //!SILVERLIGHT