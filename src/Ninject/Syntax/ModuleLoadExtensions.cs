#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.Reflection;
using Ninject.Infrastructure;
using Ninject.Modules;
#endregion

namespace Ninject
{
	/// <summary>
	/// Extension methods that enhance module loading.
	/// </summary>
	public static class ModuleLoadExtensions
	{
		/// <summary>
		/// Creates a new instance of the module and loads it into the kernel.
		/// </summary>
		/// <typeparam name="TModule">The type of the module.</typeparam>
		/// <param name="kernel">The kernel.</param>
		public static void Load<TModule>(this IKernel kernel)
			where TModule : INinjectModule, new()
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			kernel.Load(new TModule());
		}

		/// <summary>
		/// Loads the module(s) into the kernel.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="modules">The modules to load.</param>
		public static void Load(this IKernel kernel, params INinjectModule[] modules)
		{
			kernel.Load(modules);
		}

		#if !NO_ASSEMBLY_SCANNING
		/// <summary>
		/// Loads modules from the files that match the specified pattern(s).
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
		public static void Load(this IKernel kernel, params string[] filePatterns)
		{
			kernel.Load(filePatterns);
		}

		/// <summary>
		/// Loads modules defined in the specified assemblies.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="assemblies">The assemblies to search.</param>
		public static void Load(this IKernel kernel, params Assembly[] assemblies)
		{
			kernel.Load(assemblies);
		}
		#endif
	}
}
