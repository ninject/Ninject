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
#if SILVERLIGHT_20 || SILVERLIGHT_30 || WINDOWS_PHONE || NETCF || MONO
        /// <summary>
        /// The objects that were activated as reference equal weak references.
        /// </summary>
        private readonly IDictionary<object, bool> activatedObjects = new Dictionary<object, bool>();

        /// <summary>
        /// The objects that were activated as reference equal weak references.
        /// </summary>
        private readonly IDictionary<object, bool> deactivatedObjects = new Dictionary<object, bool>();
#else
        /// <summary>
        /// The objects that were activated as reference equal weak references.
        /// </summary>
        private readonly HashSet<object> activatedObjects = new HashSet<object>();

        /// <summary>
        /// The objects that were activated as reference equal weak references.
        /// </summary>
        private readonly HashSet<object> deactivatedObjects = new HashSet<object>();
#endif

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
#if SILVERLIGHT_20 || SILVERLIGHT_30 || WINDOWS_PHONE || NETCF || MONO || PCL
                this.activatedObjects.Add(new ReferenceEqualWeakReference(instance), true);
#else
                this.activatedObjects.Add(new ReferenceEqualWeakReference(instance));
#endif
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
#if SILVERLIGHT_20 || SILVERLIGHT_30 || WINDOWS_PHONE || NETCF || MONO || PCL
                this.deactivatedObjects.Add(new ReferenceEqualWeakReference(instance), true);
#else
                this.deactivatedObjects.Add(new ReferenceEqualWeakReference(instance));
#endif
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
#if SILVERLIGHT_20 || SILVERLIGHT_30 || WINDOWS_PHONE || NETCF || MONO || PCL
            return this.activatedObjects.ContainsKey(instance);
#else
            return this.activatedObjects.Contains(instance);
#endif
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
#if SILVERLIGHT_20 || SILVERLIGHT_30 || WINDOWS_PHONE || NETCF || MONO || PCL
            return this.deactivatedObjects.ContainsKey(instance);
#else
            return this.deactivatedObjects.Contains(instance);
#endif        
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

#if SILVERLIGHT_20 || SILVERLIGHT_30 || WINDOWS_PHONE || NETCF || MONO || PCL
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
#else
        /// <summary>
        /// Removes all dead objects.
        /// </summary>
        /// <param name="objects">The objects collection to be freed of dead objects.</param>
        private static void RemoveDeadObjects(HashSet<object> objects)
        {
#if WINRT
            var deadObjects = objects.Where(reference => !((ReferenceEqualWeakReference)reference).IsAlive).ToList();
            foreach (var deadObject in deadObjects)
            {
                objects.Remove(deadObject);
            }
#else
            objects.RemoveWhere(reference => !((ReferenceEqualWeakReference)reference).IsAlive);
#endif
        }
#endif
    }
}