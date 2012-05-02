#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion

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
        /// The timer used to trigger the cache pruning
        /// </summary>
        private Timer timer;

        private bool stop;

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed && this.timer != null)
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
#if !NETCF
                this.timer.Dispose(signal);
                signal.WaitOne();
#else
                this.timer.Dispose();
#endif

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
            TimeSpan interval = Settings.CachePruningInterval;
            return interval == TimeSpan.MaxValue ? -1 : (int)interval.TotalMilliseconds;
        }
    }
}