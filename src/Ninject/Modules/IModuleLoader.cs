using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Components;

namespace Ninject.Modules
{
	/// <summary>
	/// Automatically finds and loads modules from assemblies.
	/// </summary>
	public interface IModuleLoader : INinjectComponent
	{
		/// <summary>
		/// Loads all loadable modules defined in the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		void LoadModules(Assembly assembly);

		/// <summary>
		/// Loads all loadable modules defined in the assembly with the specified assembly name or filename.
		/// </summary>
		/// <param name="assemblyOrFileName">Name of the assembly or file.</param>
		void LoadModules(string assemblyOrFileName);

		/// <summary>
		/// Scans specified path for assemblies that match the specified pattern(s),
		/// and loads any modules defined therein into the kernel.
		/// </summary>
		/// <param name="path">The path to scan.</param>
		/// <param name="patterns">The patterns to match.</param>
		/// <param name="recursive">If <c>true</c>, scan all subdirectories of the path as well.</param>
		void ScanAndLoadModules(string path, IEnumerable<string> patterns, bool recursive);
	}
}