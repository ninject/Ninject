using System;
using System.Threading;
using Ninject.Infrastructure.Components;

namespace Ninject.Activation.Caching
{
	public class CachePruner : NinjectComponent, ICachePruner
	{
		private const int PollTimeoutMs = 1000;

		private static WeakReference _indicator;
		private static Timer _timer;

		public void StartPruning(ICache cache)
		{
			if (_timer != null)
				StopPruning();

			_indicator = new WeakReference(new object());
			_timer = new Timer(PruneCache, cache, PollTimeoutMs, PollTimeoutMs);
		}

		public void StopPruning()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}

			_indicator = null;
		}

		private static void PruneCache(object cache)
		{
			if (!_indicator.IsAlive)
			{
				((ICache)cache).Prune();
				_indicator.Target = new object();
			}
		}
	}
}