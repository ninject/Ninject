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

        [Fact]
        public void SelectedCtorIsUsedIfDeclared()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<Barracks>().ToConstructor(_ => new Barracks());
                kernel.Bind<IWeapon>().To<Sword>();
                kernel.Bind<IWarrior>().To<Samurai>();

                var barracks = kernel.Get<Barracks>();
                barracks.Should().NotBeNull();
                barracks.Warrior.Should().BeNull();
                barracks.Weapon.Should().BeNull();
            }
        }
    
        [Fact]
        public void SelectedCtorIsUsedIfDeclaredWithInjectedArgument()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<Barracks>().ToConstructor(ctorArg => new Barracks(ctorArg.Inject<IWarrior>()));
                kernel.Bind<IWeapon>().To<Sword>();
                kernel.Bind<IWarrior>().To<Samurai>();

                var barracks = kernel.Get<Barracks>();
                barracks.Should().NotBeNull();
                barracks.Warrior.Should().NotBeNull();
                barracks.Warrior.Should().BeOfType<Samurai>();
                barracks.Weapon.Should().BeNull();
            }
        }
    
        [Fact]
        public void WhenDefaultValuesArePassedToConstrctorSelectionTheyAreUsed()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<Barracks>().ToConstructor(ctorArg => new Barracks(new Ninja(new Sword()), ctorArg.Inject<IWeapon>()));
                kernel.Bind<IWeapon>().To<Sword>();
                kernel.Bind<IWarrior>().To<Samurai>();

                var barracks = kernel.Get<Barracks>();
                barracks.Should().NotBeNull();
                barracks.Warrior.Should().NotBeNull();
                barracks.Warrior.Should().BeOfType<Ninja>();
                barracks.Weapon.Should().NotBeNull();
            }
        }

        [Fact]
        public void DefaultValuesAreEvaluatedForEachRequest()
        {
            using (IKernel kernel = new StandardKernel())
            {
                kernel.Bind<Barracks>().ToConstructor(_ => new Barracks(new Ninja(new Sword())));

                var barracks1 = kernel.Get<Barracks>();
                var barracks2 = kernel.Get<Barracks>();

                barracks1.Warrior.Should().NotBeSameAs(barracks2.Warrior);
            }
        }

        [Fact]
        public void ConstantsCanBePassedToToConstructor()
        {
            using (IKernel kernel = new StandardKernel())
            {
                var ninja = new Ninja(new Sword());
                kernel.Bind<Barracks>().ToConstructor(_ => new Barracks(ninja));

                var barracks1 = kernel.Get<Barracks>();
                var barracks2 = kernel.Get<Barracks>();

                barracks1.Warrior.Should().BeSameAs(barracks2.Warrior);
            }
        }
        
        [Fact]
        public void WhenLazyValuesArePassedToConstrctorSelectionTheyAreEvaluatedAtResolve()
        {
            using (IKernel kernel = new StandardKernel())
            {
                int activationCount = 0;
                kernel.Bind<Ninja>().ToSelf().Named("1").OnActivation(inst => activationCount++);
                kernel.Bind<Barracks>().ToConstructor(ctorArg => new Barracks(ctorArg.Context.Kernel.Get<Ninja>("1"), ctorArg.Inject<IWeapon>()));
                kernel.Bind<IWeapon>().To<Sword>();
                kernel.Bind<IWarrior>().To<Samurai>();

                activationCount.Should().Be(0);
                var barracks = kernel.Get<Barracks>();

                barracks.Should().NotBeNull();
                barracks.Warrior.Should().NotBeNull();
                barracks.Warrior.Should().BeOfType<Ninja>();
                barracks.Weapon.Should().NotBeNull();
                activationCount.Should().Be(1);
            }
        }
    }
}