﻿namespace Ninject.Tests.Integration.ThreadScopeTests
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using Ninject.Activation.Caching;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ThreadScopeContext : IDisposable
    {
        protected StandardKernel kernel;

        public ThreadScopeContext()
        {
            var settings = new NinjectSettings { CachePruningInterval = TimeSpan.MaxValue };
            this.kernel = new StandardKernel(settings);
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenServiceIsBoundWithThreadScope : ThreadScopeContext
    {
        [Fact]
        public void FirstActivatedInstanceIsReusedWithinThread()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().InThreadScope();

            IWeapon weapon1 = null;
            IWeapon weapon2 = null;

            ThreadStart callback = () =>
            {
                weapon1 = this.kernel.Get<IWeapon>();
                weapon2 = this.kernel.Get<IWeapon>();
            };

            var thread = new Thread(callback);

            thread.Start();
            thread.Join();

            weapon1.Should().NotBeNull();
            weapon2.Should().NotBeNull();
            weapon1.Should().BeSameAs(weapon2);
        }

        [Fact]
        public void ScopeDoesNotInterfereWithExternalRequests()
        {
            this.kernel.Bind<IWeapon>().To<Sword>().InThreadScope();

            IWeapon weapon1 = this.kernel.Get<IWeapon>();
            IWeapon weapon2 = null;

            ThreadStart callback = () => weapon2 = this.kernel.Get<IWeapon>();

            var thread = new Thread(callback);

            thread.Start();
            thread.Join();

            weapon1.Should().NotBeNull();
            weapon2.Should().NotBeNull();
            weapon1.Should().NotBeSameAs(weapon2);
        }

        [Fact]
        public void InstancesActivatedWithinScopeAreDeactivatedAfterThreadIsGarbageCollectedAndCacheIsPruned()
        {
            this.kernel.Bind<NotifiesWhenDisposed>().ToSelf().InThreadScope();
            var cache = this.kernel.Components.Get<ICache>();

            // Use separate method to allow thread/scope to be finalized
            NotifiesWhenDisposed instance = GetInstanceFromSeparateThread();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            cache.Prune();

            instance.Should().NotBeNull();
            instance.IsDisposed.Should().BeTrue();
        }

        private NotifiesWhenDisposed GetInstanceFromSeparateThread()
        {
            NotifiesWhenDisposed instance = null;

            ThreadStart callback = () => instance = this.kernel.Get<NotifiesWhenDisposed>();

            var thread = new Thread(callback);

            thread.Start();
            thread.Join();

            return instance;
        }
    }
}