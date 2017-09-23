// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForType.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class ExtensionsForType
    {
        /// <summary>
        /// Gets an enumerable containing the given type and all its base types
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An enumerable containing the given type and all its base types</returns>
         public static IEnumerable<Type> GetAllBaseTypes(this Type type)
         {
             while (type != null)
             {
                 yield return type;

                 type = type.BaseType;
             }
         }
    }
}