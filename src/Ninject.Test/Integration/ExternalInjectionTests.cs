namespace Ninject.Tests.Integration.ExternalInjectionTests
{
    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ExternalInjectionContext
    {
        protected StandardKernel kernel;

        public ExternalInjectionContext()
        {
            this.kernel = new StandardKernel();
        }
    }

    public class WhenInjectIsCalled : ExternalInjectionContext
    {
        [Fact]
        public void InstanceOfKernelIsInjected()
        {
            kernel.Bind<IWeapon>().To<Sword>();

            var warrior = new ExternalWarrior();
            kernel.Inject(warrior);

            warrior.Weapon.Should().NotBeNull();
            warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void InstanceIsNotTrackedForDeactivation()
        {
            var instance = new NotifiesWhenDisposed();

            kernel.Inject(instance);
            kernel.Dispose();

            instance.IsDisposed.Should().BeFalse();
        }
    }

    public class ExternalWarrior
    {
        [Inject] public IWeapon Weapon { get; set; }
    }
}