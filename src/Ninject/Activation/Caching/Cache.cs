using System;
using System.Linq;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Disposal;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;

namespace Ninject.Activation.Caching
{
	/// <summary>
	/// Tracks instances for re-use in certain scopes.
	/// </summary>
	public class Cache : NinjectComponent, ICache
	{
		private readonly object _mutex = new object();
		private readonly Multimap<IBinding, CacheEntry> _entries = new Multimap<IBinding, CacheEntry>();

		/// <summary>
		/// Gets or sets the pipeline component.
		/// </summary>
		public IPipeline Pipeline { get; private set; }

		/// <summary>
		/// Gets or sets the cache pruner component.
		/// </summary>
		public ICachePruner Pruner { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Cache"/> class.
		/// </summary>
		/// <param name="pipeline">The pipeline component.</param>
		/// <param name="pruner">The pruner component.</param>
		public Cache(IPipeline pipeline, ICachePruner pruner)
		{
			_entries = new Multimap<IBinding, CacheEntry>();
			Pipeline = pipeline;
			Pruner = pruner;
			Pruner.StartPruning(this);
		}

		/// <summary>
		/// Releases resources held by the object.
		/// </summary>
		public override void Dispose()
		{
			if (Pruner != null)
			{
				Pruner.StopPruning();
				Pruner = null;
			}

			base.Dispose();
		}

		/// <summary>
		/// Stores the specified context in the cache.
		/// </summary>
		/// <param name="context">The context to store.</param>
		public void Remember(IContext context)
		{
			lock (_mutex)
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
			lock (_mutex)
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

						if (!cachedArguments.ElementsEqual(arguments))
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
			lock (_mutex)
			{
				foreach (IBinding binding in _entries.Keys)
					_entries[binding].Where(e => !e.Scope.IsAlive).ToArray().Map(Forget);
			}
		}

		private void Forget(CacheEntry entry)
		{
			lock (_mutex)
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