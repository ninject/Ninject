using Ninject.Tests.Fakes;
using Ninject.Tests.Integration.StandardKernelTests;
using Xunit;

namespace Ninject.Tests.Integration
{
    public class ConditionalBindingTests: StandardKernelContext
    {
        [Fact]
        public void GivenADefaultAndSingleSatisfiedConditional_ThenTheConditionalIsUsed()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Samurai>();
            kernel.Bind<Samurai>().ToSelf();
            var warrior = kernel.Get<Samurai>();
            Assert.IsType<Shuriken>(warrior.Weapon);
        }

        [Fact]
        public void GivenADefaultAndSingleUnatisfiedConditional_ThenTheDefaultIsUsed()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Ninja>();
            kernel.Bind<Samurai>().ToSelf();
            var warrior = kernel.Get<Samurai>();
            Assert.IsType<Sword>(warrior.Weapon);
        }

        [Fact]
        public void GivenADefaultAndAnUnSatisfiedConditional_ThenTheDefaultIsUsed()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Ninja>();
            kernel.Bind<Samurai>().ToSelf();
            var warrior = kernel.Get<Samurai>();
            Assert.IsType<Sword>(warrior.Weapon);
        }

        [Fact]
        public void GivenADefaultAndAnManySatisfiedConditionals_ThenAnExceptionIsThrown()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Sword>().WhenInjectedInto<Samurai>();
            kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Samurai>();
            kernel.Bind<Samurai>().ToSelf();
            Assert.Throws<ActivationException>(() => kernel.Get<Samurai>());
        }

        [Fact]
        public void GivenNoBinding_ThenASelfBindableTypeWillResolve()
        {
            var weapon = kernel.Get<Sword>();
            Assert.IsType<Sword>(weapon);
        }

        [Fact]
        public void GivenBindingIsMadeAfterImplictBinding_ThenNonImplicitBindingWillResolve()
        {
            IWeapon weapon = kernel.Get<Sword>();
            Assert.IsType<Sword>(weapon);
            kernel.Bind<Sword>().To<ShortSword>();
            weapon = kernel.Get<Sword>();
            Assert.IsType<ShortSword>(weapon);
        }
    }
}