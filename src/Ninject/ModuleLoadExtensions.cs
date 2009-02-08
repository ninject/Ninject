using System;
using System.Reflection;
using Ninject.Modules;

namespace Ninject
{
	public static class ModuleLoadExtensions
	{
		private static readonly string[] DefaultPatterns = new[] { "*.dll", "*.exe" };

		public static bool HasModule<TModule>(this IKernel kernel)
			where TModule : IModule
		{
			return kernel.HasModule(typeof(TModule));
		}

		public static void LoadModule<TModule>(this IKernel kernel)
			where TModule : IModule, new()
		{
			kernel.LoadModule(new TModule());
		}

		public static void UnloadModule<TModule>(this IKernel kernel)
			where TModule : IModule
		{
			kernel.UnloadModule(typeof(TModule));
		}

		public static void AutoLoadModules(this IKernel kernel)
		{
			GetModuleLoader(kernel).ScanAndLoadModules("~", DefaultPatterns, false);
		}

		public static void AutoLoadModules(this IKernel kernel, string path)
		{
			GetModuleLoader(kernel).ScanAndLoadModules(path, DefaultPatterns, false);
		}

		public static void AutoLoadModules(this IKernel kernel, string path, params string[] patterns)
		{
			GetModuleLoader(kernel).ScanAndLoadModules(path, patterns, false);
		}

		public static void AutoLoadModulesRecursively(this IKernel kernel)
		{
			GetModuleLoader(kernel).ScanAndLoadModules("~", DefaultPatterns, true);
		}

		public static void AutoLoadModulesRecursively(this IKernel kernel, string path)
		{
			GetModuleLoader(kernel).ScanAndLoadModules(path, DefaultPatterns, true);
		}

		public static void AutoLoadModulesRecursively(this IKernel kernel, string path, params string[] patterns)
		{
			GetModuleLoader(kernel).ScanAndLoadModules(path, patterns, true);
		}

		public static void LoadModulesFromAssembly(this IKernel kernel, Assembly assembly)
		{
			GetModuleLoader(kernel).LoadModules(assembly);
		}

		public static void LoadModulesFromAssembly(this IKernel kernel, string assemblyOrFileName)
		{
			GetModuleLoader(kernel).LoadModules(assemblyOrFileName);
		}

		private static IModuleLoader GetModuleLoader(IKernel kernel)
		{
			return kernel.Components.Get<IModuleLoader>();
		}
	}
}
