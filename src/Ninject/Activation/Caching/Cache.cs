using System;
using System.Linq;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Disposal;
using Ninject.Infrastructure.Language;
using Ninject.Planning.Bindings;

namespace Ninject.Activation.Caching
{
	public class Cache : NinjectComponent, ICache
	{
		private readonly object _mutex = new object();

		public IPipeline Pipeline { get; private set; }
		public ICachePruner Pruner { get; private set; }
		public Multimap<IBinding, CacheEntry> Entries { get; private set; }

		public Cache(IPipeline pipeline, ICachePruner pruner)
		{
			Entries = new Multimap<IBinding, CacheEntry>();
			Pipeline = pipeline;
			Pruner = pruner;
			Pruner.StartPruning(this);
		}

		public override void Dispose()
		{
			if (Pruner != null)
			{
				Pruner.StopPruning();
				Pruner = null;
			}

			base.Dispose();
		}

		public void Remember(IContext context)
		{
			lock (_mutex)
			{
				var entry = new CacheEntry(context);
				Entries[context.Binding].Add(entry);

				var scope = context.GetScope() as INotifyWhenDisposed;

				if (scope != null)
					scope.Disposed += (o, e) => Forget(entry);
			}
		}

		public object TryGet(IContext context)
		{
			lock (_mutex)
			{
				Prune();

				var scope = context.GetScope();

				foreach (CacheEntry entry in Entries[context.Binding])
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

		public void Prune()
		{
			lock (_mutex)
			{
				foreach (IBinding binding in Entries.Keys)
					Entries[binding].Where(e => !e.Scope.IsAlive).ToArray().Map(Forget);
			}
		}

		private void Forget(CacheEntry entry)
		{
			lock (_mutex)
			{
				Pipeline.Deactivate(entry.Context);
				Entries[entry.Context.Binding].Remove(entry);
			}
		}

		public class CacheEntry
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