// -------------------------------------------------------------------------------------------------
// <copyright file="GarbageCollectionCachePruner.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;

    /// <summary>
    /// Uses a <see cref="Timer"/> and some <see cref="WeakReference"/> magic to poll
    /// the garbage collector to see if it has run.
    /// </summary>
    public class GarbageCollectionCachePruner : NinjectComponent, ICachePruner
    {
        /// <summary>
        /// indicator for if GC has been run.
        /// </summary>
        private readonly WeakReference indicator = new WeakReference(new object());

        /// <summary>
        /// The caches that are being pruned.
        /// </summary>
        private readonly List<IPruneable> caches = new List<IPruneable>();

        /// <summary>
        /// The timer used to trigger the cache pruning.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// The flag to indicate whether the cache pruning is stopped or not.
        /// </summary>
        private bool stop;

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><c>True</c> if called manually, otherwise by GC.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed && this.timer != null)
            {
                this.Stop();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Starts pruning the specified pruneable based on the rules of the pruner.
        /// </summary>
        /// <param name="pruneable">The pruneable that will be pruned.</param>
        public void Start(IPruneable pruneable)
        {
            Ensure.ArgumentNotNull(pruneable, "pruneable");

            this.caches.Add(pruneable);
            if (this.timer == null)
            {
                this.timer = new Timer(this.PruneCacheIfGarbageCollectorHasRun, null, this.GetTimeoutInMilliseconds(), Timeout.Infinite);
            }
        }

        /// <summary>
        /// Stops pruning.
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                this.stop = true;
            }

            using (var signal = new ManualResetEvent(false))
            {
                this.timer.Dispose(signal);
                signal.WaitOne();
                this.timer = null;
                this.caches.Clear();
            }
        }

        private void PruneCacheIfGarbageCollectorHasRun(object state)
        {
            lock (this)
            {
                if (this.stop)
                {
                    return;
                }

                try
                {
                    if (this.indicator.IsAlive)
                    {
                        return;
                    }

                    this.caches.Map(cache => cache.Prune());
                    this.indicator.Target = new object();
                }
                finally
                {
                    this.timer.Change(this.GetTimeoutInMilliseconds(), Timeout.Infinite);
                }
            }
        }

        private int GetTimeoutInMilliseconds()
        {
            var interval = this.Settings.CachePruningInterval;
            return interval == TimeSpan.MaxValue ? -1 : (int)interval.TotalMilliseconds;
        }
    }
}