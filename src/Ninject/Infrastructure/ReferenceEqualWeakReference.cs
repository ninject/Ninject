// -------------------------------------------------------------------------------------------------
// <copyright file="ReferenceEqualWeakReference.cs" company="Ninject Project Contributors">
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
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Weak reference that can be used in collections. It is equal to the
    /// object it references and has the same hash code.
    /// </summary>
    public class ReferenceEqualWeakReference : WeakReference
    {
        private readonly int cachedHashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEqualWeakReference"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ReferenceEqualWeakReference(object target)
            : base(target)
        {
            this.cachedHashCode = RuntimeHelpers.GetHashCode(target);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEqualWeakReference"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trackResurrection">if set to <c>true</c> [track resurrection].</param>
        public ReferenceEqualWeakReference(object target, bool trackResurrection)
            : base(target, trackResurrection)
        {
            this.cachedHashCode = RuntimeHelpers.GetHashCode(target);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var thisInstance = this.IsAlive ? this.Target : this;

            if (obj is WeakReference referenceEqualWeakReference && referenceEqualWeakReference.IsAlive)
            {
                obj = referenceEqualWeakReference.Target;
            }

            return ReferenceEquals(thisInstance, obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.cachedHashCode;
        }
    }
}