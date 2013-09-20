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
        /// <summary>
        /// The objects that were activated as reference equal weak references.
        /// </summary>
        private readonly ConcurrentDictionaryEx<object, bool> activatedObjects = new ConcurrentDictionaryEx<object, bool>(new WeakReferenceEqualityComparer());

        /// <summary>
        /// The objects that were activated as reference equal weak references.
        /// </summary>
		private readonly ConcurrentDictionaryEx<object, bool> deactivatedObjects = new ConcurrentDictionaryEx<object, bool>(new WeakReferenceEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationCache"/> class.
        /// </summary>
        /// <param name="cachePruner">The cache pruner.</param>
        public ActivationCache(ICachePruner cachePruner)
        {
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
            this.activatedObjects.Clear();
            this.deactivatedObjects.Clear();
        }

        /// <summary>
        /// Adds an activated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        public void AddActivatedInstance(object instance)
        {
			this.activatedObjects.TryAdd(new ReferenceEqualWeakReference(instance), true);
        }

        /// <summary>
        /// Adds an deactivated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        public void AddDeactivatedInstance(object instance)
        {
			this.deactivatedObjects.TryAdd(new ReferenceEqualWeakReference(instance), true);
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
        private static void RemoveDeadObjects(ConcurrentDictionaryEx<object, bool> objects)
        {
	        var keysToRemove = objects.Keys.Where(reference => !((ReferenceEqualWeakReference) reference).IsAlive).ToArray();

	        foreach (var key in keysToRemove) {
		        bool ignored;
		        objects.TryRemove(key, out ignored);
	        }
        }
    }
}