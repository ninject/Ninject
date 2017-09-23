// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForAssembly.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

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
            return assembly.ExportedTypes.Any(IsLoadableModule);
        }

        /// <summary>
        /// Gets loadable <see cref="INinjectModule"/>s from the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <returns>The loadable <see cref="INinjectModule"/>s</returns>
        public static IEnumerable<INinjectModule> GetNinjectModules(this Assembly assembly)
        {
            return assembly.IsDynamic ?
                Enumerable.Empty<INinjectModule>() :
                assembly.ExportedTypes.Where(IsLoadableModule)
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