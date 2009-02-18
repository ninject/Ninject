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
using Ninject.Infrastructure.Disposal;
using Ninject.Infrastructure.Language;
#endregion

namespace Ninject.Infrastructure
{
	/// <summary>
	/// Uses a <see cref="Timer"/> to poll the garbage collector to see if it has run.
	/// </summary>
	public class GarbageCollectionWatcher : DisposableObject, IGarbageCollectionWatcher
	{
		private readonly WeakReference _indicator = new WeakReference(new object());
		private Timer _timer;

		/// <summary>
		/// Occurs when the garbage collector has run. Since the GC operation may be concurrent,
		/// collection may not be complete when this event is fired.
		/// </summary>
		public event EventHandler GarbageCollected;

		/// <summary>
		/// Initializes a new instance of the <see cref="GarbageCollectionWatcher"/> class.
		/// </summary>
		/// <param name="pollIntervalMs">The interval at which to poll the garbage collector.</param>
		public GarbageCollectionWatcher(int pollIntervalMs)
		{
			_timer = new Timer(RaiseEventIfGarbageCollectorHasRun, null, pollIntervalMs, pollIntervalMs);
		}

		/// <summary>
		/// Releases resources held by the object.
		/// </summary>
		public override void Dispose()
		{
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
			_timer.Dispose();
			_timer = null;

			GarbageCollected = null;

			base.Dispose();
		}

		private void RaiseEventIfGarbageCollectorHasRun(object state)
		{
			if (_indicator.IsAlive)
				return;

			GarbageCollected.Raise(this, EventArgs.Empty);
			_indicator.Target = new object();
		}
	}
}