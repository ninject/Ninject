namespace Ninject.Tests.Integration
{
    using FluentAssertions;
    using Ninject.Parameters;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ConstructorSelectionTests
    {
        [Fact]
        public void DefaultCtorIsUsedWhenNoBindingAreAvailable()
        {
            using ( IKernel kernel = new StandardKernel() )
            {
                kernel.Bind<Barracks>().ToSelf();

                var barracks = kernel.Get<Barracks>();
                barracks.Should().NotBeNull();
                barracks.Warrior.Should().BeNull();
                barracks.Weapon.Should().BeNull();
            }
        }

        [Fact]
        public void CtorIsUsedWhenParameterIsSupplied()
        {
            using(IKernel kernel = new StandardKernel())
            {
                kernel.Bind<Barracks>().ToSelf();
                var constructorArgument = new ConstructorArgument("warrior", new Samurai(new Sword()));
                var barracks = kernel.Get<Barracks>(constructorArgument);

                barracks.Should().NotBeNull();
                barracks.Warrior.Should().NotBeNull(); 
                barracks.Warrior.Weapon.Should().NotBeNull();
                barracks.Weapon.Should().BeNull(); 
            }
        }

        [Fact]
        public void FirstAvailableWithBindingAvailableIsUsed()
        {
            using ( IKernel kernel = new StandardKernel() )
            {
                kernel.Bind<Barracks>().ToSelf();
                kernel.Bind<IWeapon>().To<Sword>();

                var barracks = kernel.Get<Barracks>();
                barracks.Should().NotBeNull();
                barracks.Warrior.Should().BeNull();
                barracks.Weapon.Should().NotBeNull(); 
            }
        }

        [Fact]
        public void CtorWithMostDependenciesIsUsedWhenBindingsAreAvailable()
        {
            using ( IKernel kernel = new StandardKernel() )
            {
                kernel.Bind<Barracks>().ToSelf();
                kernel.Bind<IWeapon>().To<Sword>();
                kernel.Bind<IWarrior>().To<Samurai>();

                var barracks = kernel.Get<Barracks>();
                barracks.Should().NotBeNull();
                barracks.Warrior.Should().NotBeNull();
                barracks.Warrior.Weapon.Should().NotBeNull();
                barracks.Weapon.Should().NotBeNull(); 
            }
        }

        [Fact]
        public void CreationWillFailIfAllDepenciesAreMissingAndInjectAttributeIsApplied()
        {
            using ( IKernel kernel = new StandardKernel() )
            {
                kernel.Bind<NinjaBarracks>().ToSelf();

                Assert.Throws<ActivationException>( () => kernel.Get<NinjaBarracks>() );

                kernel.Bind<IWeapon>().To<Sword>();
                Assert.Throws<ActivationException>( () => kernel.Get<NinjaBarracks>() );
                kernel.Unbind<IWeapon>();

                kernel.Bind<IWarrior>().To<Samurai>();
                Assert.Throws<ActivationException>( () => kernel.Get<NinjaBarracks>() );
                kernel.Unbind<IWarrior>();
            }
        }
    }
}