using System;
using System.Threading;
using Ninject.Components;

namespace Ninject.Activation.Caching
{
	/// <summary>
	/// Periodically prunes an <see cref="ICache"/>.
	/// </summary>
	public class CachePruner : NinjectComponent, ICachePruner
	{
		private static readonly WeakReference _indicator = new WeakReference(new object());
		private Timer _timer;

		/// <summary>
		/// Starts periodically pruning the specified cache.
		/// </summary>
		/// <param name="cache">The cache to prune.</param>
		public void StartPruning(ICache cache)
		{
			if (_timer != null)
				StopPruning();

			int timeoutMs = Kernel.Settings.CachePruneTimeoutMs;
			_timer = new Timer(PruneCache, cache, timeoutMs, timeoutMs);
		}

		/// <summary>
		/// Stops the periodic pruning operation.
		/// </summary>
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