#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Linq;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Disposal;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Activation.Caching
{
	/// <summary>
	/// Tracks instances for re-use in certain scopes.
	/// </summary>
	public class Cache : NinjectComponent, ICache
	{
		private readonly Multimap<IBinding, CacheEntry> _entries = new Multimap<IBinding, CacheEntry>();

		/// <summary>
		/// Gets or sets the pipeline component.
		/// </summary>
		public IPipeline Pipeline { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Cache"/> class.
		/// </summary>
		/// <param name="pipeline">The pipeline component.</param>
		/// <param name="cachePruner">The cache pruner component.</param>
		public Cache(IPipeline pipeline, ICachePruner cachePruner)
		{
			_entries = new Multimap<IBinding, CacheEntry>();
			Pipeline = pipeline;
			cachePruner.Start(this);
		}

		/// <summary>
		/// Stores the specified context in the cache.
		/// </summary>
		/// <param name="context">The context to store.</param>
		public void Remember(IContext context)
		{
			lock (_entries)
			{
				var entry = new CacheEntry(context);
				_entries[context.Binding].Add(entry);

				var scope = context.GetScope() as INotifyWhenDisposed;

				if (scope != null)
					scope.Disposed += (o, e) => Forget(entry);
			}
		}

		/// <summary>
		/// Tries to retrieve an instance to re-use in the specified context.
		/// </summary>
		/// <param name="context">The context that is being activated.</param>
		/// <returns>The instance for re-use, or <see langword="null"/> if none has been stored.</returns>
		public object TryGet(IContext context)
		{
			lock (_entries)
			{
				Prune();

				var scope = context.GetScope();

				foreach (CacheEntry entry in _entries[context.Binding])
				{
					if (!ReferenceEquals(entry.Scope.Target, scope))
						continue;

					if (context.HasInferredGenericArguments)
					{
						var cachedArguments = entry.Context.GenericArguments;
						var arguments = context.GenericArguments;

						if (!cachedArguments.SequenceEqual(arguments))
							continue;
					}

					return entry.Context.Instance;
				}

				return null;
			}
		}

		/// <summary>
		/// Removes instances from the cache whose scopes have been garbage collected.
		/// </summary>
		public void Prune()
		{
			lock (_entries)
			{
				foreach (IBinding binding in _entries.Keys)
					_entries[binding].Where(e => !e.Scope.IsAlive).ToArray().Map(Forget);
			}
		}

		private void Forget(CacheEntry entry)
		{
			lock (_entries)
			{
				Pipeline.Deactivate(entry.Context);
				_entries[entry.Context.Binding].Remove(entry);
			}
		}

		private class CacheEntry
		{
			public IContext Context { get; set; }
			public WeakReference Scope { get; set; }

			public CacheEntry(IContext context)
			{
				Context = context;
				Scope = new WeakReference(context.GetScope());
			}
		}
	}
}