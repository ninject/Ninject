namespace Ninject.Tests.Integration
{
    using Ninject.Parameters;
    using Ninject.Tests.Fakes;
#if SILVERLIGHT
    using UnitDriven;
    using UnitDriven.Should;
    using Assert = Ninject.SilverlightTests.AssertWithThrows;
    using Fact = UnitDriven.TestMethodAttribute;
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    [TestClass]
    public class ConstructorSelectionTests
    {
        [Fact]
        public void DefaultCtorIsUsedWhenNoBindingAreAvailable()
        {
            using ( IKernel kernel = new StandardKernel() )
            {
                kernel.Bind<Barracks>().ToSelf();

                var barracks = kernel.Get<Barracks>();
                barracks.ShouldNotBeNull();
                barracks.Warrior.ShouldBeNull();
                barracks.Weapon.ShouldBeNull();
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

                barracks.ShouldNotBeNull();
                barracks.Warrior.ShouldNotBeNull(); 
                barracks.Warrior.Weapon.ShouldNotBeNull();
                barracks.Weapon.ShouldBeNull(); 
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
                barracks.ShouldNotBeNull();
                barracks.Warrior.ShouldBeNull();
                barracks.Weapon.ShouldNotBeNull(); 
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
                barracks.ShouldNotBeNull();
                barracks.Warrior.ShouldNotBeNull();
                barracks.Warrior.Weapon.ShouldNotBeNull();
                barracks.Weapon.ShouldNotBeNull(); 
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