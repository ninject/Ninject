// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForIEnumerableOfT.cs" company="Ninject Project Contributors">
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

namespace Ninject.Infrastructure.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class ExtensionsForIEnumerableOfT
    {
        /// <summary>
        /// Executes the given action for each of the elements in the enumerable.
        /// </summary>
        /// <typeparam name="T">Type of the enumerable.</typeparam>
        /// <param name="series">The series.</param>
        /// <param name="action">The action.</param>
        public static void Map<T>(this IEnumerable<T> series, Action<T> action)
        {
            foreach (T item in series)
            {
                action(item);
            }
        }

        /// <summary>
        /// Converts the given enumerable type to prevent changed on the type behind.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="series">The series.</param>
        /// <returns>The input type as real enumerable not castable to the original type.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerable<T> series)
        {
            return series.Select(x => x);
        }

        /// <summary>
        /// Returns single element of enumerable or throws exception.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="exceptionCreator">The exception creator.</param>
        /// <typeparam name="T">Type of the enumerable.</typeparam>
        /// <returns>The single element of enumerable.</returns>
        /// <exception cref="ActivationException">
        /// Exception specified by exception creator.
        /// </exception>
        public static T SingleOrThrowException<T>(this IEnumerable<T> series, Func<ActivationException> exceptionCreator)
        {
            var e = series.GetEnumerator();
            e.MoveNext();
            var result = e.Current;
            if (e.MoveNext())
            {
                throw exceptionCreator();
            }

            return result;
        }
    }
}