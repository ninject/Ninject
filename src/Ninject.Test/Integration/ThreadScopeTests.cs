namespace Ninject.Tests.Integration.ThreadScopeTests
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
            kernel.Bind<IWeapon>().To<Sword>().InThreadScope();

            IWeapon weapon1 = null;
            IWeapon weapon2 = null;

            ThreadStart callback = () =>
            {
                weapon1 = kernel.Get<IWeapon>();
                weapon2 = kernel.Get<IWeapon>();
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
            kernel.Bind<IWeapon>().To<Sword>().InThreadScope();

            IWeapon weapon1 = kernel.Get<IWeapon>();
            IWeapon weapon2 = null;

            ThreadStart callback = () => weapon2 = kernel.Get<IWeapon>();

            var thread = new Thread(callback);

            thread.Start();
            thread.Join();

            weapon1.Should().NotBeNull();
            weapon2.Should().NotBeNull();
            weapon1.Should().NotBeSameAs(weapon2);
        }

#if !MONO
        [Fact]
        public void InstancesActivatedWithinScopeAreDeactivatedAfterThreadIsGarbageCollectedAndCacheIsPruned()
        {
            kernel.Bind<NotifiesWhenDisposed>().ToSelf().InThreadScope();
            var cache = kernel.Components.Get<ICache>();

            NotifiesWhenDisposed instance = null;

            ThreadStart callback = () => instance = kernel.Get<NotifiesWhenDisposed>();

            var thread = new Thread(callback);

            thread.Start();
            thread.Join();

            thread = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            cache.Prune();

            instance.Should().NotBeNull();
            instance.IsDisposed.Should().BeTrue();
        }
#endif
    }
}