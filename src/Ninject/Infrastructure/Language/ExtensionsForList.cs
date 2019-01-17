// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForList.cs" company="Ninject Project Contributors">
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

    using Enumerable = System.Linq.Enumerable;

    /// <summary>
    /// Provides extension methods for <see cref="IList{T}"/> and <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    internal static class ExtensionsForList
    {
        /// <summary>
        /// Produces the set union of two lists by using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input lists.</typeparam>
        /// <param name="first">An <see cref="IReadOnlyList{T}"/> whose distinct elements form the first set for the union.</param>
        /// <param name="second">An <see cref="ICollection{T}"/> whose distinct elements form the second set for the union.</param>
        /// <returns>
        /// An <see cref="IReadOnlyList{T}"/> that contains the elements from both input lists, excluding duplicates.
        /// </returns>
        /// <exception cref="NullReferenceException"><paramref name="second"/> is <see langword="null"/>.</exception>
        /// <exception cref="NullReferenceException"><paramref name="second"/> is not empy, and <paramref name="first"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<T> Union<T>(this IReadOnlyList<T> first, ICollection<T> second)
        {
            var numberItemsInSecond = second.Count;

            if (numberItemsInSecond == 0)
            {
                if (first.Count == 0)
                {
                    return first;
                }
                else
                {
                    var set = new HashSet<T>(first);
                    var union = new T[set.Count];
                    set.CopyTo(union);
                    return union;
                }
            }
            else
            {
                if (first.Count == 0)
                {
                    var set = new HashSet<T>(first);
                    var union = new T[set.Count];
                    set.CopyTo(union);
                    return union;
                }

                return Enumerable.ToList(Enumerable.Union(first, second));
            }
        }

        /// <summary>
        /// Concatenates two lists.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input lists.</typeparam>
        /// <param name="first">The first list to concatenate.</param>
        /// <param name="second">The second list to concatenate.</param>
        /// <returns>
        /// An <see cref="IReadOnlyList{T}"/> that contains the concatenated elements of the two input lists.
        /// </returns>
        /// <exception cref="NullReferenceException"><paramref name="second"/> is <see langword="null"/>.</exception>
        /// <exception cref="NullReferenceException"><paramref name="second"/> is not empy, and <paramref name="first"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// When <paramref name="second"/> is empty, then <paramref name="first"/> will be returned as is (without making
        /// a copy).
        /// </remarks>
        public static IReadOnlyList<T> Concat<T>(this IReadOnlyList<T> first, IList<T> second)
        {
            var numberItemsInSecond = second.Count;

            if (numberItemsInSecond == 0)
            {
                return first;
            }
            else
            {
                T[] union;
                var numberItemsInFirst = first.Count;

                if (numberItemsInFirst == 0)
                {
                    union = new T[numberItemsInSecond];
                    for (var i = 0; i < numberItemsInSecond; i++)
                    {
                        union[i] = second[i];
                    }
                }
                else
                {
                    union = new T[numberItemsInFirst + numberItemsInSecond];

                    for (var i = 0; i < numberItemsInFirst; i++)
                    {
                        union[i] = first[i];
                    }

                    for (var i = 0; i < numberItemsInSecond; i++)
                    {
                        union[i + numberItemsInFirst] = second[i];
                    }
                }

                return union;
            }
        }
    }
}