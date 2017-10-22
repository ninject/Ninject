// -------------------------------------------------------------------------------------------------
// <copyright file="AssemblyNameRetriever.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Modules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Ninject.Components;

    /// <summary>
    /// Retrieves assembly names from file names using a temporary app domain.
    /// </summary>
    public class AssemblyNameRetriever : NinjectComponent, IAssemblyNameRetriever
    {
        /// <summary>
        /// Gets all assembly names of the assemblies in the given files that match the filter.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>All assembly names of the assemblies in the given files that match the filter.</returns>
        public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames, Predicate<Assembly> filter)
        {
#if !NO_ASSEMBLY_SCANNING
            var assemblyCheckerType = typeof(AssemblyChecker);
            var temporaryDomain = CreateTemporaryAppDomain();
            try
            {
                var checker = (AssemblyChecker)temporaryDomain.CreateInstanceAndUnwrap(
                    assemblyCheckerType.Assembly.FullName,
                    assemblyCheckerType.FullName ?? string.Empty);

                return checker.GetAssemblyNames(filenames.ToArray(), filter);
            }
            finally
            {
                AppDomain.Unload(temporaryDomain);
            }
#else
            return new AssemblyChecker().GetAssemblyNames(filenames, filter);
#endif
        }

#if !NO_ASSEMBLY_SCANNING
        /// <summary>
        /// Creates a temporary app domain.
        /// </summary>
        /// <returns>The created app domain.</returns>
        private static AppDomain CreateTemporaryAppDomain()
        {
            return AppDomain.CreateDomain(
                "NinjectModuleLoader",
                AppDomain.CurrentDomain.Evidence,
                AppDomain.CurrentDomain.SetupInformation);
        }
#endif

        /// <summary>
        /// This class is loaded into the temporary appdomain to load and check if the assemblies match the filter.
        /// </summary>
        private class AssemblyChecker : MarshalByRefObject
        {
            /// <summary>
            /// Gets the assembly names of the assemblies matching the filter.
            /// </summary>
            /// <param name="filenames">The filenames.</param>
            /// <param name="filter">The filter.</param>
            /// <returns>All assembly names of the assemblies matching the filter.</returns>
            public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames, Predicate<Assembly> filter)
            {
                var result = new List<AssemblyName>();
                foreach (var filename in filenames)
                {
                    Assembly assembly;
                    if (File.Exists(filename))
                    {
                        try
                        {
                            assembly = Assembly.LoadFrom(filename);
                        }
                        catch (BadImageFormatException)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        try
                        {
                            assembly = Assembly.Load(filename);
                        }
                        catch (FileLoadException)
                        {
                            continue;
                        }
                        catch (FileNotFoundException)
                        {
                            continue;
                        }
                    }

                    if (filter(assembly))
                    {
                        result.Add(assembly.GetName(false));
                    }
                }

                return result;
            }
        }
    }
}