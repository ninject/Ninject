//-------------------------------------------------------------------------------
// <copyright file="ExtensionsForType.cs" company="Ninject Project Contributors">
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

namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for type
    /// </summary>
    /// <remarks></remarks>
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