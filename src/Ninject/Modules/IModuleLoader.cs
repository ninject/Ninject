using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Components;

namespace Ninject.Modules
{
	public interface IModuleLoader : INinjectComponent
	{
		void LoadModules(Assembly assembly);
		void LoadModules(string assemblyOrFileName);
		void ScanAndLoadModules(string path, IEnumerable<string> patterns, bool recursive);
	}
}