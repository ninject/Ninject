// -------------------------------------------------------------------------------------------------
// <copyright file="Cache.cs" company="Ninject Project Contributors">
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
        private readonly ConcurrentDictionary<object, ConcurrentDictionary<IBindingConfiguration, HashSet<CacheEntry>>> entries =
           new ConcurrentDictionary<object, ConcurrentDictionary<IBindingConfiguration, HashSet<CacheEntry>>>(new WeakReferenceEqualityComparer());

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
            get { return this.GetAllCacheEntries().Count(); }
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
                           notifyScope.Disposed += (o, e) => this.Clear(weakScopeReference);
                       }

                       return new ConcurrentDictionary<IBindingConfiguration, HashSet<CacheEntry>>();
                   });

            scopedEntries.GetOrAdd(context.Binding.BindingConfiguration, new HashSet<CacheEntry>()).Add(entry);
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

            if (!this.entries.TryGetValue(scope, out ConcurrentDictionary<IBindingConfiguration, HashSet<CacheEntry>> bindings))
            {
                return null;
            }

            foreach (var entry in bindings.TryGetValue(context.Binding.BindingConfiguration, out HashSet<CacheEntry> entries) ? entries : Enumerable.Empty<CacheEntry>())
            {
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

            return null;
        }

        /// <summary>
        /// Deactivates and releases the specified instance from the cache.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        /// <see langword="true"/> if the instance was found and released; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Release(object instance)
        {
            var instanceFound = false;
            foreach (var bindingEntry in this.entries.Values.SelectMany(bindingEntries => bindingEntries.Values).ToList())
            {
                var instanceEntries = bindingEntry.Where(cacheEntry => ReferenceEquals(instance, cacheEntry.Reference.Instance)).ToList();
                foreach (var cacheEntry in instanceEntries)
                {
                    this.Forget(cacheEntry);
                    bindingEntry.Remove(cacheEntry);
                    instanceFound = true;
                }
            }

            return instanceFound;
        }

        /// <summary>
        /// Removes instances from the cache which should no longer be re-used.
        /// </summary>
        public void Prune()
        {
            var disposedScopes = this.entries.Where(scope => !((ReferenceEqualWeakReference)scope.Key).IsAlive).Select(scope => scope).ToList();
            foreach (var disposedScope in disposedScopes)
            {
                this.Clear(disposedScope.Key);
            }
        }

        /// <summary>
        /// Immediately deactivates and removes all instances in the cache that are owned by
        /// the specified scope.
        /// </summary>
        /// <param name="scope">The scope whose instances should be deactivated.</param>
        public void Clear(object scope)
        {
            if (this.entries.TryRemove(scope, out ConcurrentDictionary<IBindingConfiguration, HashSet<CacheEntry>> bindings))
            {
                this.Forget(bindings.SelectMany(b => b.Value));
            }
        }

        /// <summary>
        /// Immediately deactivates and removes all instances in the cache, regardless of scope.
        /// </summary>
        public void Clear()
        {
            this.Forget(this.GetAllCacheEntries());
            this.entries.Clear();
        }

        /// <summary>
        /// Gets all entries for a binding within the selected scope.
        /// </summary>
        /// <param name="bindings">The bindings.</param>
        /// <returns>All bindings of a binding.</returns>
        private static IEnumerable<CacheEntry> GetAllBindingEntries(ConcurrentDictionary<IBindingConfiguration, HashSet<CacheEntry>> bindings)
        {
            return bindings.Values.SelectMany(bindingEntries => bindingEntries);
        }

        /// <summary>
        /// Gets all cache entries.
        /// </summary>
        /// <returns>Returns all cache entries.</returns>
        private IEnumerable<CacheEntry> GetAllCacheEntries()
        {
            return this.entries.SelectMany(scopeCache => GetAllBindingEntries(scopeCache.Value));
        }

        /// <summary>
        /// Forgets the specified cache entries.
        /// </summary>
        /// <param name="cacheEntries">The cache entries.</param>
        private void Forget(IEnumerable<CacheEntry> cacheEntries)
        {
            foreach (var entry in cacheEntries.ToList())
            {
                this.Forget(entry);
            }
        }

        /// <summary>
        /// Forgets the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        private void Forget(CacheEntry entry)
        {
            this.Clear(entry.Reference.Instance);
            this.Pipeline.Deactivate(entry.Context, entry.Reference);
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