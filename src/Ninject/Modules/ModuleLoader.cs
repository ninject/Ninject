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
using System.Reflection;
using System.Web;
using Ninject.Components;
#endregion

namespace Ninject.Modules
{
	/// <summary>
	/// Automatically finds and loads modules from assemblies.
	/// </summary>
	public class ModuleLoader : NinjectComponent, IModuleLoader
	{
		/// <summary>
		/// Loads all loadable modules defined in the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		public void LoadModules(Assembly assembly)
		{
			foreach (Type type in assembly.GetExportedTypes().Where(IsLoadableModule))
			{
				if (Kernel.HasModule(type))
					continue;

				var module = Activator.CreateInstance(type) as IModule;
				Kernel.LoadModule(module);
			}
		}

		/// <summary>
		/// Loads all loadable modules defined in the assembly with the specified assembly name or filename.
		/// </summary>
		/// <param name="assemblyOrFileName">Name of the assembly or file.</param>
		public void LoadModules(string assemblyOrFileName)
		{
			AssemblyName name;

			try
			{
				name = new AssemblyName(assemblyOrFileName);
			}
			catch (ArgumentException)
			{
				name = new AssemblyName { CodeBase = assemblyOrFileName };
			}

			LoadModules(Assembly.Load(name));
		}

		/// <summary>
		/// Scans specified path for assemblies that match the specified pattern(s),
		/// and loads any modules defined therein into the kernel.
		/// </summary>
		/// <param name="path">The path to scan.</param>
		/// <param name="patterns">The patterns to match.</param>
		/// <param name="recursive">If <c>true</c>, scan all subdirectories of the path as well.</param>
		public void ScanAndLoadModules(string path, IEnumerable<string> patterns, bool recursive)
		{
			var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var normalizedPath = NormalizePath(path);
			var files = patterns.SelectMany(pattern => Directory.GetFiles(normalizedPath, pattern, searchOption));

			AppDomain temporaryDomain = CreateTemporaryAppDomain();

			foreach (AssemblyName assemblyName in FindAssembliesWithModules(temporaryDomain, files))
				LoadModules(Assembly.Load(assemblyName));

			AppDomain.Unload(temporaryDomain);
		}

		/// <summary>
		/// Scans the specified series of files for assemblies that contain loadable modules.
		/// </summary>
		/// <param name="temporaryDomain">The temporary <see cref="AppDomain"/> to load assemblies in to check them.</param>
		/// <param name="files">The files to check.</param>
		/// <returns>The names of the assemblies that contain loadable modules.</returns>
		protected virtual IEnumerable<AssemblyName> FindAssembliesWithModules(AppDomain temporaryDomain, IEnumerable<string> files)
		{
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
		}

		/// <summary>
		/// Determines whether the specified type represents a loadable module.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><c>True</c> if the type represents a loadable module; otherwise <c>false</c>.</returns>
		protected virtual bool IsLoadableModule(Type type)
		{
			if (!typeof(IModule).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
				return false;

			return type.GetConstructor(Type.EmptyTypes) != null;
		}

		/// <summary>
		/// Normalizes the provided path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>The normalized path.</returns>
		protected virtual string NormalizePath(string path)
		{
			if (path.StartsWith("~"))
				path = GetBaseDirectory() + path.Substring(1);

			return new DirectoryInfo(path).FullName;
		}

		private static string GetBaseDirectory()
		{
			return HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~") : AppDomain.CurrentDomain.BaseDirectory;
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