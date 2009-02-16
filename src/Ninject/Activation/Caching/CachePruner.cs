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
#endregion

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