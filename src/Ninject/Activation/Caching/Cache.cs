#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
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
		/// Gets the number of entries currently stored in the cache.
		/// </summary>
		public int Count
		{
			get { return _entries.Sum(list => list.Value.Count); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Cache"/> class.
		/// </summary>
		/// <param name="pipeline">The pipeline component.</param>
		/// <param name="cachePruner">The cache pruner component.</param>
		public Cache(IPipeline pipeline, ICachePruner cachePruner)
		{
			Ensure.ArgumentNotNull(pipeline, "pipeline");
			Ensure.ArgumentNotNull(cachePruner, "cachePruner");

			_entries = new Multimap<IBinding, CacheEntry>();
			Pipeline = pipeline;
			cachePruner.Start(this);
		}

		/// <summary>
		/// Releases resources held by the object.
		/// </summary>
		public override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed)
				Clear();

			base.Dispose(disposing);
		}

		/// <summary>
		/// Stores the specified context in the cache.
		/// </summary>
		/// <param name="context">The context to store.</param>
		/// <param name="reference">The instance reference.</param>
		public void Remember(IContext context, InstanceReference reference)
		{
			Ensure.ArgumentNotNull(context, "context");

			var entry = new CacheEntry(context, reference);
		
			lock (_entries)
				_entries[context.Binding].Add(entry);

			var notifyScope = context.GetScope() as INotifyWhenDisposed;

			if (notifyScope != null)
				notifyScope.Disposed += (o, e) => Forget(entry);
		}

		/// <summary>
		/// Tries to retrieve an instance to re-use in the specified context.
		/// </summary>
		/// <param name="context">The context that is being activated.</param>
		/// <returns>The instance for re-use, or <see langword="null"/> if none has been stored.</returns>
		public object TryGet(IContext context)
		{
			Ensure.ArgumentNotNull(context, "context");

			lock (_entries)
			{
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

					return entry.Reference.Instance;
				}

				return null;
			}
		}

		/// <summary>
		/// Deactivates and releases the specified instance from the cache.
		/// </summary>
		/// <param name="instance">The instance to release.</param>
		/// <returns><see langword="True"/> if the instance was found and released; otherwise <see langword="false"/>.</returns>
		public bool Release(object instance)
		{
			return ForgetAllWhere(e => ReferenceEquals(instance, e.Reference.Instance));
		}

		/// <summary>
		/// Removes instances from the cache which should no longer be re-used.
		/// </summary>
		public void Prune()
		{
			ForgetAllWhere(e => !e.Scope.IsAlive);
		}

		/// <summary>
		/// Immediately deactivates and removes all instances in the cache that are owned by
		/// the specified scope.
		/// </summary>
		/// <param name="scope">The scope whose instances should be deactivated.</param>
		public void Clear(object scope)
		{
			ForgetAllWhere(e => ReferenceEquals(scope, e.Scope.Target));
		}

		/// <summary>
		/// Immediately deactivates and removes all instances in the cache, regardless of scope.
		/// </summary>
		public void Clear()
		{
			ForgetAllWhere(e => true);
		}

		private bool ForgetAllWhere(Func<CacheEntry, bool> predicate)
		{
			lock (_entries)
			{
				var matchingEntries = _entries.SelectMany(e => e.Value).Where(predicate).ToArray();
				matchingEntries.Map(Forget);
				return matchingEntries.Any();
			}
		}

		private void Forget(CacheEntry entry)
		{
			Pipeline.Deactivate(entry.Context, entry.Reference);
			_entries[entry.Context.Binding].Remove(entry);
		}

		private class CacheEntry
		{
			public IContext Context { get; private set; }
			public InstanceReference Reference { get; private set; }
			public WeakReference Scope { get; private set; }

			public CacheEntry(IContext context, InstanceReference reference)
			{
				Context = context;
				Reference = reference;
				Scope = new WeakReference(context.GetScope());
			}
		}
	}
}