// -------------------------------------------------------------------------------------------------
// <copyright file="Cache.cs" company="Ninject Project Contributors">
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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// Tracks instances for re-use in certain scopes.
    /// </summary>
    public class Cache : NinjectComponent, ICache
    {
        /// <summary>
        /// Contains all cached instances.
        /// This is a dictionary of scopes to a multimap for bindings to cache entries.
        /// </summary>
        private readonly ConcurrentDictionary<object, ConcurrentDictionary<IBindingConfiguration, List<CacheEntry>>> entries =
           new ConcurrentDictionary<object, ConcurrentDictionary<IBindingConfiguration, List<CacheEntry>>>(new WeakReferenceEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache"/> class.
        /// </summary>
        /// <param name="pipeline">The pipeline component.</param>
        /// <param name="cachePruner">The cache pruner component.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pipeline"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="cachePruner"/> is <see langword="null"/>.</exception>
        public Cache(IPipeline pipeline, ICachePruner cachePruner)
        {
            Ensure.ArgumentNotNull(pipeline, nameof(pipeline));
            Ensure.ArgumentNotNull(cachePruner, nameof(cachePruner));

            this.Pipeline = pipeline;
            cachePruner.Start(this);
        }

        /// <summary>
        /// Gets the pipeline component.
        /// </summary>
        public IPipeline Pipeline { get; private set; }

        /// <summary>
        /// Gets the number of entries currently stored in the cache.
        /// </summary>
        public int Count
        {
            get { return this.GetAllCacheEntries().Count; }
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if called manually, otherwise by GC.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this.Clear();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Stores the specified context in the cache.
        /// </summary>
        /// <param name="context">The context to store.</param>
        /// <param name="reference">The instance reference.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        public void Remember(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            var scope = context.GetScope();
            var entry = new CacheEntry(context, reference);

            var weakScopeReference = new ReferenceEqualWeakReference(scope);

            var scopedEntries = this.entries.GetOrAdd(
                   weakScopeReference,
                   key =>
                   {
                       if (scope is INotifyWhenDisposed notifyScope)
                       {
                           notifyScope.Disposed += (o, e) => this.Clear(key);
                       }

                       return new ConcurrentDictionary<IBindingConfiguration, List<CacheEntry>>();
                   });

            var cacheEntriesForBinding = scopedEntries.GetOrAdd(context.Binding.BindingConfiguration, new List<CacheEntry>());

            lock (cacheEntriesForBinding)
            {
                cacheEntriesForBinding.Add(entry);
            }
        }

        /// <summary>
        /// Stores the specified context in the cache.
        /// </summary>
        /// <param name="context">The context to store.</param>
        /// <param name="scope">The scope of the context.</param>
        /// <param name="reference">The instance reference.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="scope"/> is <see langword="null"/>.</exception>
        public void Remember(IContext context, object scope, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, nameof(context));
            Ensure.ArgumentNotNull(scope, nameof(scope));

            var entry = new CacheEntry(context, reference);
            var weakScopeReference = new ReferenceEqualWeakReference(scope);

            var scopedEntries = this.entries.GetOrAdd(
                   weakScopeReference,
                   key =>
                   {
                       // to reduce allocations, we use the argument of the delegate instead of
                       // using the "scope" argument directly
                       var scopeToAdd = ((ReferenceEqualWeakReference)key).Target;
                       if (scopeToAdd is INotifyWhenDisposed notifyScope)
                       {
                           notifyScope.Disposed += (o, e) => this.Clear(scopeToAdd);
                       }

                       return new ConcurrentDictionary<IBindingConfiguration, List<CacheEntry>>();
                   });

            var cacheEntriesForBinding = scopedEntries.GetOrAdd(context.Binding.BindingConfiguration, new List<CacheEntry>());

            lock (cacheEntriesForBinding)
            {
                cacheEntriesForBinding.Add(entry);
            }
        }

        /// <summary>
        /// Tries to retrieve an instance to re-use in the specified context.
        /// </summary>
        /// <param name="context">The context that is being activated.</param>
        /// <returns>
        /// The instance for re-use, or <see langword="null"/> if none has been stored.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        public object TryGet(IContext context)
        {
            Ensure.ArgumentNotNull(context, nameof(context));

            var scope = context.GetScope();
            if (scope == null)
            {
                return null;
            }

            if (!this.entries.TryGetValue(scope, out ConcurrentDictionary<IBindingConfiguration, List<CacheEntry>> bindings))
            {
                return null;
            }

            if (bindings.TryGetValue(context.Binding.BindingConfiguration, out List<CacheEntry> cacheEntriesForBinding))
            {
                lock (cacheEntriesForBinding)
                {
                    var entryCount = cacheEntriesForBinding.Count;
                    for (var i = 0; i < entryCount; i++)
                    {
                        var entry = cacheEntriesForBinding[i];
                        if (context.HasInferredGenericArguments)
                        {
                            var cachedArguments = entry.Context.GenericArguments;
                            var arguments = context.GenericArguments;

                            if (!cachedArguments.SequenceEqual(arguments))
                            {
                                continue;
                            }
                        }

                        return entry.Reference.Instance;
                    }
                }
            }

            return null;
        }

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
        public object TryGet(IContext context, object scope)
        {
            Ensure.ArgumentNotNull(context, nameof(context));
            Ensure.ArgumentNotNull(scope, nameof(scope));

            if (!this.entries.TryGetValue(scope, out ConcurrentDictionary<IBindingConfiguration, List<CacheEntry>> bindings))
            {
                return null;
            }

            if (bindings.TryGetValue(context.Binding.BindingConfiguration, out List<CacheEntry> cacheEntriesForBinding))
            {
                lock (cacheEntriesForBinding)
                {
                    var entryCount = cacheEntriesForBinding.Count;
                    for (var i = 0; i < entryCount; i++)
                    {
                        var entry = cacheEntriesForBinding[i];
                        if (context.HasInferredGenericArguments)
                        {
                            var cachedArguments = entry.Context.GenericArguments;
                            var arguments = context.GenericArguments;

                            if (!cachedArguments.SequenceEqual(arguments))
                            {
                                continue;
                            }
                        }

                        return entry.Reference.Instance;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Deactivates and releases the specified instance from the cache.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        /// <see langword="true"/> if the instance was found and released; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// To improve concurrency we first compose the list of cache entries for the specified instance,
        /// and only then - without holding a lock - deactivate these cache entries and clear any scope
        /// for the instance.
        /// </remarks>
        public bool Release(object instance)
        {
            List<CacheEntry> cacheEntriesForInstance = null;

            foreach (var entry in this.entries)
            {
                foreach (var bindingEntry in entry.Value)
                {
                    var cacheEntriesForBinding = bindingEntry.Value;
                    lock (cacheEntriesForBinding)
                    {
                        var cacheEntryCount = cacheEntriesForBinding.Count;
                        for (var i = cacheEntryCount - 1; i >= 0; i--)
                        {
                            var cacheEntry = cacheEntriesForBinding[i];
                            if (ReferenceEquals(instance, cacheEntry.Reference.Instance))
                            {
                                if (cacheEntriesForInstance == null)
                                {
                                    cacheEntriesForInstance = new List<CacheEntry>();
                                }

                                cacheEntriesForInstance.Add(cacheEntry);
                                cacheEntriesForBinding.RemoveAt(i);
                            }
                        }
                    }
                }
            }

            if (cacheEntriesForInstance != null)
            {
                foreach (var cacheEntry in cacheEntriesForInstance)
                {
                    // only deactivate cache entry, we'll be clearing any scope we have for it later
                    this.Pipeline.Deactivate(cacheEntry.Context, cacheEntry.Reference);
                }

                // TODO: discuss whether this is actually necessary as:
                // * it's unlikely that a cached instance itself is a scope
                // * if there were a corresponding scope, then removing all instances from the
                //   would allow the scope to be finalized. This means the scope itself would
                //   be removed from the cache upon the next pruning run.
                //
                // Note that this will throw an ArgumentNullException if instance is null, but
                // Ninject itself will never add a null instance to the cache.
                this.Clear(instance);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes scopes that have been finalized from the cache.
        /// </summary>
        public void Prune()
        {
            foreach (var finalizedScope in this.GetFinalizedScopes())
            {
                this.Clear(finalizedScope);
            }
        }

        /// <summary>
        /// Immediately deactivates and removes all instances in the cache that are owned by
        /// the specified scope.
        /// </summary>
        /// <param name="scope">The scope whose instances should be deactivated.</param>
        public void Clear(object scope)
        {
            if (this.entries.TryRemove(scope, out ConcurrentDictionary<IBindingConfiguration, List<CacheEntry>> bindings))
            {
                foreach (var binding in bindings)
                {
                    var cacheEntriesForBinding = binding.Value;

                    lock (cacheEntriesForBinding)
                    {
                        foreach (var cacheEntry in cacheEntriesForBinding)
                        {
                            this.Pipeline.Deactivate(cacheEntry.Context, cacheEntry.Reference);

                            // TODO: discuss whether this is actually necessary as:
                            // * it's unlikely that a cached instance itself is a scope
                            // * if there were a corresponding scope, then removing all instances from the
                            //   would allow the scope to be finalized. This means the scope itself would
                            //   be removed from the cache upon the next pruning run.
                            //
                            // Note that this will throw an ArgumentNullException if instance is null, but
                            // Ninject itself will never add a null instance to the cache.
                            this.Clear(cacheEntry.Reference.Instance);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Immediately deactivates and removes all instances in the cache, regardless of scope.
        /// </summary>
        public void Clear()
        {
            var cacheEntries = this.GetAllCacheEntries();
            this.entries.Clear();
            this.Deactivate(cacheEntries);
        }

        /// <summary>
        /// Gets all cache entries.
        /// </summary>
        /// <returns>
        /// All cache entries.
        /// </returns>
        private List<CacheEntry> GetAllCacheEntries()
        {
            var allCacheEntries = new List<CacheEntry>();

            foreach (var scopeEntry in this.entries)
            {
                foreach (var bindingEntry in scopeEntry.Value)
                {
                    var cacheEntriesForBinding = bindingEntry.Value;

                    lock (cacheEntriesForBinding)
                    {
                        foreach (var cacheEntry in cacheEntriesForBinding)
                        {
                            allCacheEntries.Add(cacheEntry);
                        }
                    }
                }
            }

            return allCacheEntries;
        }

        /// <summary>
        /// Deactivates the specified cache entries.
        /// </summary>
        /// <param name="cacheEntries">The cache entries.</param>
        private void Deactivate(List<CacheEntry> cacheEntries)
        {
            foreach (var entry in cacheEntries)
            {
                this.Pipeline.Deactivate(entry.Context, entry.Reference);
            }
        }

        /// <summary>
        /// Returns a list of finalizes scoped for which we currently still have an entry in our cache.
        /// </summary>
        /// <returns>
        /// The finalized scopes for which we current still have an entry in our cache.
        /// </returns>
        private List<ReferenceEqualWeakReference> GetFinalizedScopes()
        {
            var finalizedScopes = new List<ReferenceEqualWeakReference>();

            foreach (var entry in this.entries)
            {
                var scopeReference = (ReferenceEqualWeakReference)entry.Key;
                if (!scopeReference.IsAlive)
                {
                    finalizedScopes.Add(scopeReference);
                }
            }

            return finalizedScopes;
        }

        /// <summary>
        /// An entry in the cache.
        /// </summary>
        private class CacheEntry
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CacheEntry"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="reference">The instance reference.</param>
            public CacheEntry(IContext context, InstanceReference reference)
            {
                this.Context = context;
                this.Reference = reference;
            }

            /// <summary>
            /// Gets the context of the instance.
            /// </summary>
            /// <value>The context.</value>
            public IContext Context { get; private set; }

            /// <summary>
            /// Gets the instance reference.
            /// </summary>
            /// <value>The instance reference.</value>
            public InstanceReference Reference { get; private set; }
        }
    }
}