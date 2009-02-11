using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Ninject.Components;
using Ninject.Syntax;

namespace Ninject.Modules
{
	public class ModuleLoader : NinjectComponent, IModuleLoader
	{
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

		public void ScanAndLoadModules(string path, IEnumerable<string> patterns, bool recursive)
		{
			var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var normalizedPath = NormalizePath(path);
			var files = patterns.SelectMany(pattern => Directory.GetFiles(normalizedPath, pattern, searchOption));

			foreach (AssemblyName assemblyName in FindAssembliesWithModules(files))
				LoadModules(Assembly.Load(assemblyName));
		}

		protected virtual IEnumerable<AssemblyName> FindAssembliesWithModules(IEnumerable<string> files)
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

		protected virtual bool IsLoadableModule(Type type)
		{
			if (!typeof(IModule).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
				return false;

			return type.GetConstructor(Type.EmptyTypes) != null;
		}

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