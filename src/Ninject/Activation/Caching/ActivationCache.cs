// -------------------------------------------------------------------------------------------------
// <copyright file="ActivationCache.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Caching
{
    using System.Collections.Generic;

    using Ninject.Components;
    using Ninject.Infrastructure;

    /// <summary>
    /// Stores the objects that were activated
    /// </summary>
    public class ActivationCache : NinjectComponent, IActivationCache, IPruneable
    {
        /// <summary>
        /// The objects that were activated as reference equal weak references.
        /// </summary>
        private readonly HashSet<object> activatedObjects = new HashSet<object>(new WeakReferenceEqualityComparer());

        /// <summary>
        /// The objects that were activated as reference equal weak references.
        /// </summary>
        private readonly HashSet<object> deactivatedObjects = new HashSet<object>(new WeakReferenceEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationCache"/> class.
        /// </summary>
        /// <param name="cachePruner">The cache pruner.</param>
        public ActivationCache(ICachePruner cachePruner)
        {
            Ensure.ArgumentNotNull(cachePruner, "cachePruner");
            cachePruner.Start(this);
        }

        /// <summary>
        /// Gets the activated object count.
        /// </summary>
        /// <value>The activated object count.</value>
        public int ActivatedObjectCount
        {
            get
            {
                return this.activatedObjects.Count;
            }
        }

        /// <summary>
        /// Gets the deactivated object count.
        /// </summary>
        /// <value>The deactivated object count.</value>
        public int DeactivatedObjectCount
        {
            get
            {
                return this.deactivatedObjects.Count;
            }
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            lock (this.activatedObjects)
            {
                this.activatedObjects.Clear();
            }

            lock (this.deactivatedObjects)
            {
                this.deactivatedObjects.Clear();
            }
        }

        /// <summary>
        /// Adds an activated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        public void AddActivatedInstance(object instance)
        {
            lock (this.activatedObjects)
            {
                this.activatedObjects.Add(new ReferenceEqualWeakReference(instance));
            }
        }

        /// <summary>
        /// Adds an deactivated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        public void AddDeactivatedInstance(object instance)
        {
            lock (this.deactivatedObjects)
            {
                this.deactivatedObjects.Add(new ReferenceEqualWeakReference(instance));
            }
        }

        /// <summary>
        /// Determines whether the specified instance is activated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is activated; otherwise, <c>false</c>.
        /// </returns>
        public bool IsActivated(object instance)
        {
            return this.activatedObjects.Contains(instance);
        }

        /// <summary>
        /// Determines whether the specified instance is deactivated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is deactivated; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDeactivated(object instance)
        {
            return this.deactivatedObjects.Contains(instance);
        }

        /// <summary>
        /// Prunes this instance.
        /// </summary>
        public void Prune()
        {
            lock (this.activatedObjects)
            {
                RemoveDeadObjects(this.activatedObjects);
            }

            lock (this.deactivatedObjects)
            {
                RemoveDeadObjects(this.deactivatedObjects);
            }
        }

        /// <summary>
        /// Removes all dead objects.
        /// </summary>
        /// <param name="objects">The objects collection to be freed of dead objects.</param>
        private static void RemoveDeadObjects(HashSet<object> objects)
        {
            objects.RemoveWhere(reference => !((ReferenceEqualWeakReference)reference).IsAlive);
        }
    }
}