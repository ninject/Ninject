namespace Ninject.Activation.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ninject.Components;
    using Ninject.Infrastructure;

    /// <summary>
    /// Stores the objects that were activated
    /// </summary>
    public class ActivationCache : NinjectComponent, IActivationCache, IPruneable
    {
        private readonly IActivationCacheImpl cacheImpl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationCache"/> class.
        /// </summary>
        /// <param name="cachePruner">The cache pruner.</param>
        public ActivationCache(ICachePruner cachePruner)
        {
            cachePruner.Start(this);

#if SILVERLIGHT_20 || SILVERLIGHT_30 || WINDOWS_PHONE || NETCF
            cacheImpl = new DictionaryBasedActivationCacheImpl();
#else
            if (RuntimeEnvironment.IsMonoRuntime)
            {
                cacheImpl = new DictionaryBasedActivationCacheImpl();
            }
            else
            {
                cacheImpl = new HashSetBasedActivationCacheImpl();
            }
#endif
        }

        /// <summary>
        /// Gets the activated object count.
        /// </summary>
        /// <value>The activated object count.</value>
        public int ActivatedObjectCount
        {
            get
            {
                return cacheImpl.ActivatedObjectCount;
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
                return cacheImpl.DeactivatedObjectCount;
            }
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            cacheImpl.Clear();
        }

        /// <summary>
        /// Adds an activated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        public void AddActivatedInstance(object instance)
        {
            cacheImpl.AddActivatedInstance(instance);
        }

        /// <summary>
        /// Adds an deactivated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        public void AddDeactivatedInstance(object instance)
        {
            cacheImpl.AddDeactivatedInstance(instance);
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
            return cacheImpl.IsActivated(instance);
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
            return cacheImpl.IsDeactivated(instance);
        }

        /// <summary>
        /// Prunes this instance.
        /// </summary>
        public void Prune()
        {
            cacheImpl.Prune();
        }

        private interface IActivationCacheImpl
        {
            /// <summary>
            /// Gets the activated object count.
            /// </summary>
            /// <value>The activated object count.</value>
            int ActivatedObjectCount { get; }

            /// <summary>
            /// Gets the deactivated object count.
            /// </summary>
            /// <value>The deactivated object count.</value>
            int DeactivatedObjectCount { get; }

            /// <summary>
            /// Clears the cache.
            /// </summary>
            void Clear();

            /// <summary>
            /// Adds an activated instance.
            /// </summary>
            /// <param name="instance">The instance to be added.</param>
            void AddActivatedInstance(object instance);

            /// <summary>
            /// Adds an deactivated instance.
            /// </summary>
            /// <param name="instance">The instance to be added.</param>
            void AddDeactivatedInstance(object instance);

            /// <summary>
            /// Determines whether the specified instance is activated.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <returns>
            ///     <c>true</c> if the specified instance is activated; otherwise, <c>false</c>.
            /// </returns>
            bool IsActivated(object instance);

            /// <summary>
            /// Determines whether the specified instance is deactivated.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <returns>
            ///     <c>true</c> if the specified instance is deactivated; otherwise, <c>false</c>.
            /// </returns>
            bool IsDeactivated(object instance);

            /// <summary>
            /// Prunes this instance.
            /// </summary>
            void Prune();
        }

        private class HashSetBasedActivationCacheImpl : IActivationCacheImpl
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

        private class DictionaryBasedActivationCacheImpl : IActivationCacheImpl
        {
            /// <summary>
            /// The objects that were activated as reference equal weak references.
            /// </summary>
            private readonly IDictionary<object, bool> activatedObjects = new Dictionary<object, bool>(new WeakReferenceEqualityComparer());

            /// <summary>
            /// The objects that were activated as reference equal weak references.
            /// </summary>
            private readonly IDictionary<object, bool> deactivatedObjects = new Dictionary<object, bool>(new WeakReferenceEqualityComparer());

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
                    this.activatedObjects.Add(new ReferenceEqualWeakReference(instance), true);
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
                    this.deactivatedObjects.Add(new ReferenceEqualWeakReference(instance), true);
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
                return this.activatedObjects.ContainsKey(instance);
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
                return this.deactivatedObjects.ContainsKey(instance);
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
            private static void RemoveDeadObjects(IDictionary<object, bool> objects)
            {
                var deadObjects = objects.Where(entry => !((ReferenceEqualWeakReference)entry.Key).IsAlive).ToList();
                foreach (var deadObject in deadObjects)
                {
                    objects.Remove(deadObject.Key);
                }
            }
        }
    }
}
