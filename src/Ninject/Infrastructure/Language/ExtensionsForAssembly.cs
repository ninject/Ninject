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

    internal static class ExtensionsForAssembly
    {
        public static bool HasNinjectModules(this Assembly assembly)
        {
            return assembly.GetExportedTypes().Any(IsLoadableModule);
        }

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