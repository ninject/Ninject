namespace Ninject.Tests.Integration.ExternalInjectionTests
{
    using System;

    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ExternalInjectionContext : IDisposable
    {
        protected StandardKernel kernel;

        public ExternalInjectionContext()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }
    }

    public class WhenInjectIsCalled : ExternalInjectionContext
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void InstanceOfKernelIsInjected()
        {
            kernel.Bind<IWeapon>().To<Sword>();

            var warrior = new ExternalWarrior();
            kernel.Inject(warrior);

            warrior.Weapon.Should().NotBeNull();
            warrior.Weapon.Should().BeOfType<Sword>();
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
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