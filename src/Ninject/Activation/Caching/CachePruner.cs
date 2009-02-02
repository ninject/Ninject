using System;
using System.Threading;
using Ninject.Components;

namespace Ninject.Activation.Caching
{
	public class CachePruner : NinjectComponent, ICachePruner
	{
		private static WeakReference _indicator = new WeakReference(new object());
		private Timer _timer;

		public void StartPruning(ICache cache)
		{
			if (_timer != null)
				StopPruning();

			int timeoutMs = Settings.CachePruneTimeoutMs;
			_timer = new Timer(PruneCache, cache, timeoutMs, timeoutMs);
		}

		public void StopPruning()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
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