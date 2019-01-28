// -------------------------------------------------------------------------------------------------
// <copyright file="ReferenceEqualityTypeComparer.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2019 Ninject Project Contributors. All rights reserved.
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
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// An <see cref="IEqualityComparer{Type}"/> that considers two instances equal when they are
    /// the same instances.
    /// </summary>
    public sealed class ReferenceEqualityTypeComparer : IEqualityComparer<Type>
    {
        /// <summary>
        /// Determines whether the specified instances are the same instance.
        /// </summary>
        /// <param name="x">The first instance to compare.</param>
        /// <param name="y">The second instance to compare.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="x"/> is the same instance as <paramref name="y"/> or
        /// if both are <see langword="null"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(Type x, Type y)
        {
            return ReferenceEquals(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="obj">The instance for which a hash code is to be returned.</param>
        /// <returns>
        /// A hash code for <paramref name="obj"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <see langword="null"/>.</exception>
        public int GetHashCode(Type obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}