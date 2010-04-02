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
using System.Threading;
using Ninject.Components;
using Ninject.Infrastructure;
#endregion

namespace Ninject.Activation.Caching
{
	/// <summary>
	/// Uses a <see cref="Timer"/> and some <see cref="WeakReference"/> magic to poll
	/// the garbage collector to see if it has run.
	/// </summary>
	public class GarbageCollectionCachePruner : NinjectComponent, ICachePruner
	{
		private readonly WeakReference _indicator = new WeakReference(new object());
		private Timer _timer;

		/// <summary>
		/// Gets the cache that is being pruned.
		/// </summary>
		public ICache Cache { get; private set; }

		/// <summary>
		/// Releases resources held by the object.
		/// </summary>
		public override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed && _timer != null)
				Stop();
				

			base.Dispose(disposing);
		}

		/// <summary>
		/// Starts pruning the specified cache based on the rules of the pruner.
		/// </summary>
		/// <param name="cache">The cache that will be pruned.</param>
		public void Start(ICache cache)
		{
			Ensure.ArgumentNotNull(cache, "cache");

			if (_timer != null)
				Stop();

			Cache = cache;
			_timer = new Timer(PruneCacheIfGarbageCollectorHasRun, null, GetTimeoutInMilliseconds(), Timeout.Infinite);
		}

		/// <summary>
		/// Stops pruning.
		/// </summary>
		public void Stop()
		{
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
			_timer.Dispose();
			_timer = null;
			Cache = null;
		}

		private void PruneCacheIfGarbageCollectorHasRun(object state)
		{
			try
			{
				if (_indicator.IsAlive)
					return;

				Cache.Prune();
				_indicator.Target = new object();
			}
			finally
			{
				_timer.Change(GetTimeoutInMilliseconds(), Timeout.Infinite);
			}
		}

		private int GetTimeoutInMilliseconds()
		{
			TimeSpan interval = Settings.CachePruningInterval;
			return interval == TimeSpan.MaxValue ? -1 : (int)interval.TotalMilliseconds;
		}
	}
}