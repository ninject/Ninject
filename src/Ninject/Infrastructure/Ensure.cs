// -------------------------------------------------------------------------------------------------
// <copyright file="Ensure.cs" company="Ninject Project Contributors">
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

namespace Ninject.Infrastructure
{
    using System;

    /// <summary>
    /// Argument guard.
    /// </summary>
    internal static class Ensure
    {
        /// <summary>
        /// Ensures the argument is not null.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/>.</exception>
        internal static void ArgumentNotNull(object argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name, "Cannot be null");
            }
        }

        /// <summary>
        /// Ensures the argument is not null or empty.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="ArgumentException"><paramref name="argument"/> is <see langword="null"/> or a zero-length <see cref="string"/>.</exception>
        internal static void ArgumentNotNullOrEmpty(string argument, string name)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException("Cannot be null or empty", name);
            }
        }
    }
}