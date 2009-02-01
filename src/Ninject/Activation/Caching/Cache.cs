using System;
using System.Linq;
using Ninject.Infrastructure;
using Ninject.Syntax;

namespace Ninject.Activation.Caching
{
	public class Cache : IDisposable, ICache
	{
		private readonly Multimap<Type, CacheEntry> _entries = new Multimap<Type, CacheEntry>();
		private readonly object _mutex = new object();

		public IPipeline Pipeline { get; set; }
		public ICachePruner Pruner { get; set; }

		public Cache(IPipeline pipeline, ICachePruner pruner)
		{
			Pipeline = pipeline;
			Pruner = pruner;
			Pruner.StartPruning(this);
		}

		public void Dispose()
		{
			Pruner.StopPruning();
			Pruner = null;
			GC.SuppressFinalize(this);
		}

		public void Remember(IContext context)
		{
			lock (_mutex)
			{
				var entry = new CacheEntry(context);
				_entries[context.Implementation].Add(entry);

				var scope = context.GetScope() as INotifyWhenDisposed;

				if (scope != null)
					scope.Disposed += (o, e) => Forget(entry);
			}
		}

		public object TryGet(Type type, object scope)
		{
			lock (_mutex)
			{
				Prune();
				return _entries[type].Where(x => ReferenceEquals(x.Scope.Target, scope)).Select(x => x.Context.Instance).SingleOrDefault();
			}
		}

		public void Prune()
		{
			lock (_mutex)
			{
				foreach (Type type in _entries.Keys)
					_entries[type].Where(e => !e.Scope.IsAlive).ToArray().Map(Forget);
			}
		}

		private void Forget(CacheEntry entry)
		{
			lock (_mutex)
			{
				Pipeline.Deactivate(entry.Context);
				_entries[entry.Context.Implementation].Remove(entry);
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