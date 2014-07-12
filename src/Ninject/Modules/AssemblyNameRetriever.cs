//-------------------------------------------------------------------------------
// <copyright file="AssemblyNameRetriever.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Remo Gloor (remo.gloor@gmail.com)
//           
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
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
//-------------------------------------------------------------------------------

#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Modules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Ninject.Components;

#if WINRT
    using System.Threading.Tasks;
#endif

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
        public
#if !WINRT
        IEnumerable<AssemblyName> 
#else
 System.Threading.Tasks.Task<IEnumerable<AssemblyName>>
#endif
            GetAssemblyNames(IEnumerable<string> filenames, Predicate<Assembly> filter)
        {
#if PCL
            throw new NotImplementedException();
#else
#if !WINRT
            var assemblyCheckerType = typeof(AssemblyChecker);
            var temporaryDomain = CreateTemporaryAppDomain();
            try
            {
                var checker = (AssemblyChecker)temporaryDomain.CreateInstanceAndUnwrap(
                    assemblyCheckerType.Assembly.FullName,
                    assemblyCheckerType.FullName ?? string.Empty);

                return checker.GetAssemblyNames(filenames.ToArray(), filter);
#else
                var checker = new AssemblyCheckerWinRT();
                return checker.GetAssemblyListAsync(filenames.ToArray(), filter);
#endif

#if !WINRT
            }
            finally
            {
                AppDomain.Unload(temporaryDomain);
            }
#endif
#endif
        }

#if !PCL
#if !WINRT
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
#else
        private sealed class AssemblyCheckerWinRT
        {
            //public IEnumerable<AssemblyName> GetAssemblyNames(IEnumerable<string> filenames, Predicate<Assembly> filter)
            //{
            //    return GetAssemblyListAsync(filenames, filter).Result;
            //}

            public async Task<IEnumerable<AssemblyName>> GetAssemblyListAsync(IEnumerable<string> filenames, Predicate<Assembly> filter)
            {
                var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

                var result = new List<AssemblyName>();
                var files = (await folder.GetFilesAsync()).ToDictionary(k => k.Name.ToLowerInvariant(), e => (Windows.Storage.StorageFile)e);
                
                foreach (var filename in filenames)
                {
                    Assembly assembly;
                    
                    if (files.ContainsKey(filename.ToLowerInvariant()))
                    {
                        try
                        {
                            AssemblyName name = new AssemblyName() { Name = files[filename.ToLowerInvariant()].DisplayName };
                            assembly = Assembly.Load(name);
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
                            AssemblyName name = new AssemblyName() { Name = filename };
                            assembly = Assembly.Load(name);
                        }
                        catch (FileNotFoundException)
                        {
                            continue;
                        }
                    }

                    if (filter(assembly))
                    {
#if !WINRT
                        result.Add(assembly.GetName());
#else
                        result.Add(new AssemblyName() {Name = assembly.GetName().Name});
#endif
                    }
                }

                return result;
            }
        }
#endif
#endif
    }
}
#endif