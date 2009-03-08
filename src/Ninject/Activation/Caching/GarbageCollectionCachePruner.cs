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
			_timer = new Timer(PruneCacheIfGarbageCollectorHasRun, null, Settings.CachePruningIntervalMs, Timeout.Infinite);
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
			if (_indicator.IsAlive)
				return;

			Cache.Prune();
			_indicator.Target = new object();

			_timer.Change(Settings.CachePruningIntervalMs, Timeout.Infinite);
		}
	}
}