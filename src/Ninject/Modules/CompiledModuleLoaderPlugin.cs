#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#if !NO_ASSEMBLY_SCANNING
#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;
#endregion

namespace Ninject.Modules
{
    /// <summary>
    /// Loads modules from compiled assemblies.
    /// </summary>
    public class CompiledModuleLoaderPlugin : NinjectComponent, IModuleLoaderPlugin
    {
        private static readonly string[] Extensions = new[] { ".dll" };

        /// <summary>
        /// Gets or sets the kernel into which modules will be loaded.
        /// </summary>
        public IKernel Kernel { get; private set; }

        /// <summary>
        /// Gets the file extensions that the plugin understands how to load.
        /// </summary>
        public IEnumerable<string> SupportedExtensions
        {
            get { return Extensions; }
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
            Kernel.Load(FindAssembliesWithModules(filenames).Select(name => Assembly.Load(name)));
        }

        private static IEnumerable<AssemblyName> FindAssembliesWithModules(IEnumerable<string> filenames)
        {
            var temporaryDomain = CreateTemporaryAppDomain();
            var moduleCheckerType = typeof(ModuleChecker);
            try
            {
                var checker = (ModuleChecker)temporaryDomain.CreateInstanceAndUnwrap(
                    moduleCheckerType.Assembly.FullName, moduleCheckerType.FullName ?? String.Empty);

                foreach (var name in filenames.Select(file => checker.CheckModule(file)).Where(name => name != null))
                {
                    yield return name;
                }
            }
            finally
            {
                AppDomain.Unload(temporaryDomain);
            }
        }

        private class ModuleChecker : MarshalByRefObject
        {
            public AssemblyName CheckModule(string filename)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(filename);
                    if (assembly.HasNinjectModules())
                    {
                        return assembly.GetName(false);
                    }
                }
                catch (BadImageFormatException)
                {
                    // Ignore native assemblies
                }

                return null;
            }
        }

        private static AppDomain CreateTemporaryAppDomain()
        {
            return AppDomain.CreateDomain(
                "NinjectModuleLoader",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);
        }
    }
}
#endif //!NO_ASSEMBLY_SCANNING