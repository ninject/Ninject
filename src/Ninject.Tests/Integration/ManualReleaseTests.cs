namespace Ninject.Tests.Integration.ManualReleaseTests
{
    using Ninject.Tests.Fakes;
#if SILVERLIGHT
    using UnitDriven;
    using UnitDriven.Should;
    using Fact = UnitDriven.TestMethodAttribute;
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif
    
    public class ManualReleaseContext
    {
        protected StandardKernel kernel;

        public ManualReleaseContext()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.kernel = new StandardKernel();            
        }
    }

    [TestClass]
    public class WhenReleaseIsCalled : ManualReleaseContext
    {
        [Fact]
        public void InstanceIsDeactivated()
        {
            kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

            var instance = kernel.Get<NotifiesWhenDisposed>();
            kernel.Release(instance);

            instance.IsDisposed.ShouldBeTrue();
        }

        [Fact]
        public void InstanceIsRemovedFromCache()
        {
            kernel.Bind<NotifiesWhenDisposed>().ToSelf().InSingletonScope();

            var instance1 = kernel.Get<NotifiesWhenDisposed>();
            var instance2 = kernel.Get<NotifiesWhenDisposed>();
            instance1.ShouldBeSameAs(instance2);

            kernel.Release(instance1);

            var instance3 = kernel.Get<NotifiesWhenDisposed>();
            instance3.ShouldNotBeSameAs(instance1);
            instance3.ShouldNotBeSameAs(instance2);
        }
    }
}