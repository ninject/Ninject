namespace Ninject.Tests.Integration.ThreadScopeTests
{
    using System;
    using System.Threading;
    using Ninject.Activation.Caching;
    using Ninject.Tests.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Assert = Ninject.SilverlightTests.AssertWithThrows;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Assert = Ninject.SilverlightTests.AssertWithThrows;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    public class ThreadScopeContext
    {
        protected StandardKernel kernel;

        public ThreadScopeContext()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            var settings = new NinjectSettings { CachePruningInterval = TimeSpan.MaxValue };
            this.kernel = new StandardKernel(settings);
        }
    }

    [TestClass]
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

            weapon1.ShouldNotBeNull();
            weapon2.ShouldNotBeNull();
            weapon1.ShouldBeSameAs(weapon2);
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

            weapon1.ShouldNotBeNull();
            weapon2.ShouldNotBeNull();
            weapon1.ShouldNotBeSameAs(weapon2);
        }

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

            instance.ShouldNotBeNull();
            instance.IsDisposed.ShouldBeTrue();
        }
    }
}