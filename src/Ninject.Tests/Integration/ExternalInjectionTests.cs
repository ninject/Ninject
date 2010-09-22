namespace Ninject.Tests.Integration.ExternalInjectionTests
{
    using Ninject.Tests.Fakes;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    public class ExternalInjectionContext
    {
        protected StandardKernel kernel;

        public ExternalInjectionContext()
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
    public class WhenInjectIsCalled : ExternalInjectionContext
    {
        [Fact]
        public void InstanceOfKernelIsInjected()
        {
            kernel.Bind<IWeapon>().To<Sword>();

            var warrior = new ExternalWarrior();
            kernel.Inject(warrior);

            warrior.Weapon.ShouldNotBeNull();
            warrior.Weapon.ShouldBeInstanceOf<Sword>();
        }

        [Fact]
        public void InstanceIsNotTrackedForDeactivation()
        {
            var instance = new NotifiesWhenDisposed();

            kernel.Inject(instance);
            kernel.Dispose();

            instance.IsDisposed.ShouldBeFalse();
        }
    }

    public class ExternalWarrior
    {
        [Inject] public IWeapon Weapon { get; set; }
    }
}