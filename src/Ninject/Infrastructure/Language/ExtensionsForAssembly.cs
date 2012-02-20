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
using Ninject.Modules;
#endregion

namespace Ninject.Infrastructure.Language
{
    internal static class ExtensionsForAssembly
    {
        public static bool HasNinjectModules(this Assembly assembly)
        {
            return
#if !WINRT
                assembly.GetExportedTypes()
#else
                assembly.ExportedTypes
#endif
                .Any(IsLoadableModule);
        }

        public static IEnumerable<INinjectModule> GetNinjectModules(this Assembly assembly)
        {
            return 
#if !WINRT
                assembly.GetExportedTypes()
#else
                assembly.ExportedTypes
#endif
                    .Where(IsLoadableModule)
                    .Select(type => Activator.CreateInstance(type) as INinjectModule);
        }

        private static bool IsLoadableModule(Type type)
        {
#if !WINRT
            return typeof(INinjectModule).IsAssignableFrom(type)
                && !type.IsAbstract
                && !type.IsInterface
                && type.GetConstructor(Type.EmptyTypes) != null;
#else
            var typeInfo = type.GetTypeInfo();
            return typeof(INinjectModule).GetTypeInfo().IsAssignableFrom(typeInfo)
                && !typeInfo.IsAbstract
                && !typeInfo.IsInterface
                && typeInfo.DeclaredConstructors.Where(c => !c.IsStatic && c.GetParameters().Length == 0).Any();
#endif
        }
    }
}
#endif //!NO_ASSEMBLY_SCANNING