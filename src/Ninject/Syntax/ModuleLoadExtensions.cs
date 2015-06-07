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
        /// <param name="kernelConfiguration">The kernel configuration into which the module is loaded.</param>
        public static void Load<TModule>(this IKernelConfiguration kernelConfiguration)
            where TModule : INinjectModule, new()
        {
            kernelConfiguration.Load(new TModule());
        }

        /// <summary>
        /// Loads the module(s) into the kernel.
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration.</param>
        /// <param name="modules">The modules to load into which the modules are loaded.</param>
        public static void Load(this IKernelConfiguration kernelConfiguration, params INinjectModule[] modules)
        {
            kernelConfiguration.Load(modules);
        }

        #if !NO_ASSEMBLY_SCANNING
        /// <summary>
        /// Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration into which the files are loaded.</param>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        public static void Load(this IKernelConfiguration kernelConfiguration, params string[] filePatterns)
        {
            kernelConfiguration.Load(filePatterns);
        }

        /// <summary>
        /// Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="kernelConfiguration">The kernel configuration into which the assemblies are loaded.</param>
        /// <param name="assemblies">The assemblies to search.</param>
        public static void Load(this IKernelConfiguration kernelConfiguration, params Assembly[] assemblies)
        {
            kernelConfiguration.Load(assemblies);
        }
        #endif
    }
}
