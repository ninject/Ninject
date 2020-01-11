// -------------------------------------------------------------------------------------------------
// <copyright file="ReverseComparer.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2020 Ninject Project Contributors. All rights reserved.
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

namespace Ninject.Infrastructure
{
    using System.Collections.Generic;

    /// <summary>
    /// An <see cref="IComparer{T}"/> that can be used to reverse the result of a given
    /// <see cref="IComparer{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    internal class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseComparer{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public ReverseComparer(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>:
        /// <list type="bullet">
        ///     <item>If less than 0, <paramref name="y"/>  is less than <paramref name="x"/> .</item>
        ///     <item>If 0, <paramref name="x"/> equals <paramref name="y"/>.</item>
        ///     <item>If greater than 0, <paramref name="y"/>  is greater than <paramref name="x"/> .</item>
        /// </list>
        /// </returns>
        public int Compare(T x, T y)
        {
            return this.comparer.Compare(y, x);
        }
    }
}