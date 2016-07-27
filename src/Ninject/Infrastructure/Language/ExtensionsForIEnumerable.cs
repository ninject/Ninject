//-------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForIEnumerable.cs" company="Ninject Project Contributors">
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
namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    internal static class ExtensionsForIEnumerable
    {
        public static IEnumerable CastSlow(this IEnumerable series, Type elementType)
        {
            var method = typeof(Enumerable).GetTypeInfo().GetMethod("Cast").MakeGenericMethod(elementType);
            return method.Invoke(null, new[] { series }) as IEnumerable;
        }

        public static Array ToArraySlow(this IEnumerable series, Type elementType)
        {
            var method = typeof(Enumerable).GetTypeInfo().GetMethod("ToArray").MakeGenericMethod(elementType);
            return method.Invoke(null, new[] { series }) as Array;
        }

        public static IList ToListSlow(this IEnumerable series, Type elementType)
        {
            var method = typeof(Enumerable).GetTypeInfo().GetMethod("ToList").MakeGenericMethod(elementType);
            return method.Invoke(null, new[] { series }) as IList;
        }
    }
}