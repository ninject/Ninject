//-------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForAssembly.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
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
//-------------------------------------------------------------------------------------------------
#if !NO_ASSEMBLY_SCANNING
namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Ninject.Modules;

    /// <summary>
    /// Provides extension methods for <see cref="Assembly"/>.
    /// </summary>
    internal static class ExtensionsForAssembly
    {
        /// <summary>
        /// Determines whether the assembly has loadable <see cref="INinjectModule"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <returns><c>True</c> if there's any loadable <see cref="INinjectModule"/>, otherwise <c>False</c>.</returns>
        public static bool HasNinjectModules(this Assembly assembly)
        {
            return assembly.GetExportedTypes().Any(IsLoadableModule);
        }

        /// <summary>
        /// Gets loadable <see cref="INinjectModule"/>s from the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <returns>The loadable <see cref="INinjectModule"/>s</returns>
        public static IEnumerable<INinjectModule> GetNinjectModules(this Assembly assembly)
        {
            return assembly.GetExportedTypes()
                    .Where(IsLoadableModule)
                    .Select(type => Activator.CreateInstance(type) as INinjectModule);
        }

        private static bool IsLoadableModule(Type type)
        {
            return typeof(INinjectModule).IsAssignableFrom(type)
                && !type.IsAbstract
                && !type.IsInterface
                && type.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
#endif //!NO_ASSEMBLY_SCANNING