// -------------------------------------------------------------------------------------------------
// <copyright file="ICache.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Caching
{
    using System;

    using Ninject.Components;

    /// <summary>
    /// Tracks instances for re-use in certain scopes.
    /// </summary>
    public interface ICache : INinjectComponent, IPruneable
    {
        /// <summary>
        /// Gets the number of entries currently stored in the cache.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Stores the specified instance in the cache.
        /// </summary>
        /// <param name="context">The context to store.</param>
        /// <param name="reference">The instance reference.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        void Remember(IContext context, InstanceReference reference);

        /// <summary>
        /// Stores the specified context in the cache.
        /// </summary>
        /// <param name="context">The context to store.</param>
        /// <param name="scope">The scope of the context.</param>
        /// <param name="reference">The instance reference.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="scope"/> is <see langword="null"/>.</exception>
        void Remember(IContext context, object scope, InstanceReference reference);

        /// <summary>
        /// Tries to retrieve an instance to re-use in the specified context.
        /// </summary>
        /// <param name="context">The context that is being activated.</param>
        /// <returns>
        /// The instance for re-use, or <see langword="null"/> if none has been stored.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        object TryGet(IContext context);

        /// <summary>
        /// Tries to retrieve an instance to re-use in the specified context and scope.
        /// </summary>
        /// <param name="context">The context that is being activated.</param>
        /// <param name="scope">The scope in which the instance is being activated.</param>
        /// <returns>
        /// The instance for re-use, or <see langword="null"/> if none has been stored.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="scope"/> is <see langword="null"/>.</exception>
        object TryGet(IContext context, object scope);

        /// <summary>
        /// Deactivates and releases the specified instance from the cache.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        /// <see langword="true"/> if the instance was found and released; otherwise, <see langword="false"/>.
        /// </returns>
        bool Release(object instance);

        /// <summary>
        /// Immediately deactivates and removes all instances in the cache that are owned by
        /// the specified scope.
        /// </summary>
        /// <param name="scope">The scope whose instances should be deactivated.</param>
        void Clear(object scope);

        /// <summary>
        /// Immediately deactivates and removes all instances in the cache, regardless of scope.
        /// </summary>
        void Clear();
    }
}