using System;
using System.Linq;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Disposal;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject.Activation.Caching
{
	public class Cache : NinjectComponent, ICache
	{
		private readonly Multimap<IBinding, CacheEntry> _entries = new Multimap<IBinding, CacheEntry>();
		private readonly object _mutex = new object();

		public IPipeline Pipeline { get; set; }
		public ICachePruner Pruner { get; set; }

		public Cache(IPipeline pipeline, ICachePruner pruner)
		{
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
				_entries[context.Binding].Add(entry);

				var scope = context.GetScope() as INotifyWhenDisposed;

				if (scope != null)
					scope.Disposed += (o, e) => Forget(entry);
			}
		}

		public object TryGet(IBinding binding, object scope)
		{
			lock (_mutex)
			{
				Prune();
				return _entries[binding].Where(x => ReferenceEquals(x.Scope.Target, scope)).Select(x => x.Context.Instance).SingleOrDefault();
			}
		}

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