namespace Ninject.Tests.Integration
{
    using System;
    using System.Runtime.InteropServices;

    using FluentAssertions;
    using Ninject.Parameters;
    using Ninject.Tests.Fakes;
    using Ninject.Tests.Integration.StandardKernelTests;

    using Xunit;

    public class ConstructorSelectionTests : IDisposable
    {
        private readonly StandardKernel kernel;

        public ConstructorSelectionTests()
        {
            this.kernel = new StandardKernel();
        }

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void DefaultCtorIsUsedWhenNoBindingAreAvailable()
        {
            this.kernel.Bind<Barracks>().ToSelf();

            var barracks = this.kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().BeNull();
        }

        [Fact]
        public void CtorIsUsedWhenParameterIsSupplied()
        {
            this.kernel.Bind<Barracks>().ToSelf();
            var constructorArgument = new ConstructorArgument("warrior", new Samurai(new Sword()));
            var barracks = this.kernel.Get<Barracks>(constructorArgument);

            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().BeNull();
        }

        [Fact]
        public void FirstAvailableWithBindingAvailableIsUsed()
        {
            this.kernel.Bind<Barracks>().ToSelf();
            this.kernel.Bind<IWeapon>().To<Sword>();

            var barracks = this.kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().NotBeNull();
        }

        [Fact]
        public void UnsatisfiedConditionalShouldBeIngored()
        {
            this.kernel.Bind<Barracks>().ToSelf();
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWarrior>().To<Samurai>().When(_ => false);

            var barracks = this.kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().NotBeNull();
        }

        [Fact]
        public void CtorWithMostDependenciesIsUsedWhenBindingsAreAvailable()
        {
            this.kernel.Bind<Barracks>().ToSelf();
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var barracks = this.kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Weapon.Should().NotBeNull();
            barracks.Weapon.Should().NotBeNull();
        }

        [Fact]
        public void CreationWillFailIfAllDependenciesAreMissingAndInjectAttributeIsApplied()
        {
            this.kernel.Bind<NinjaBarracks>().ToSelf();

            Assert.Throws<ActivationException>(() => this.kernel.Get<NinjaBarracks>());

            this.kernel.Bind<IWeapon>().To<Sword>();
            Assert.Throws<ActivationException>(() => this.kernel.Get<NinjaBarracks>());
            this.kernel.Unbind<IWeapon>();

            this.kernel.Bind<IWarrior>().To<Samurai>();
            Assert.Throws<ActivationException>(() => this.kernel.Get<NinjaBarracks>());
            this.kernel.Unbind<IWarrior>();
        }

        [Fact]
        public void SelectedCtorIsUsedIfDeclared()
        {
            this.kernel.Bind<Barracks>().ToConstructor(_ => new Barracks());
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var barracks = this.kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().BeNull();
            barracks.Weapon.Should().BeNull();
        }

        [Fact]
        public void SelectedCtorIsUsedIfDeclaredWithInjectedArgument()
        {
            this.kernel.Bind<Barracks>().ToConstructor(ctorArg => new Barracks(ctorArg.Inject<IWarrior>()));
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var barracks = this.kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<Samurai>();
            barracks.Weapon.Should().BeNull();
        }

        [Fact]
        public void WhenDefaultValuesArePassedToConstructorSelectionTheyAreUsed()
        {
            this.kernel.Bind<Barracks>().ToConstructor(ctorArg => new Barracks(new Ninja(new Sword()), ctorArg.Inject<IWeapon>()));
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWarrior>().To<Samurai>();

            var barracks = this.kernel.Get<Barracks>();
            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<Ninja>();
            barracks.Weapon.Should().NotBeNull();
        }

        [Fact]
        public void DefaultValuesAreEvaluatedForEachRequest()
        {
            this.kernel.Bind<Barracks>().ToConstructor(_ => new Barracks(new Ninja(new Sword())));

            var barracks1 = this.kernel.Get<Barracks>();
            var barracks2 = this.kernel.Get<Barracks>();

            barracks1.Warrior.Should().NotBeSameAs(barracks2.Warrior);
        }

        [Fact]
        public void ConstantsCanBePassedToToConstructor()
        {
            var ninja = new Ninja(new Sword());
            this.kernel.Bind<Barracks>().ToConstructor(_ => new Barracks(ninja));

            var barracks1 = this.kernel.Get<Barracks>();
            var barracks2 = this.kernel.Get<Barracks>();

            barracks1.Warrior.Should().BeSameAs(barracks2.Warrior);
        }

        private static Ninja CreateNinja()
        {
            return new Ninja(new Sword());
        }

        [Fact]
        public void ResultsFromNonGenericMethodCallsCanBePassedToToConstructor()
        {
            this.kernel.Bind<Barracks>().ToConstructor(_ => new Barracks(CreateNinja()));

            var barracks1 = this.kernel.Get<Barracks>();
            var barracks2 = this.kernel.Get<Barracks>();

            barracks1.Warrior.Should().NotBeSameAs(barracks2.Warrior);
        }

        [Fact]
        public void WhenLazyValuesArePassedToConstructorSelectionTheyAreEvaluatedAtResolve()
        {
            int activationCount = 0;
            this.kernel.Bind<Ninja>().ToSelf().Named("1").OnActivation(inst => activationCount++);
            this.kernel.Bind<Barracks>().ToConstructor(ctorArg => new Barracks(ctorArg.Context.Kernel.Get<Ninja>("1"), ctorArg.Inject<IWeapon>()));
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWarrior>().To<Samurai>();

            activationCount.Should().Be(0);
            var barracks = this.kernel.Get<Barracks>();

            barracks.Should().NotBeNull();
            barracks.Warrior.Should().NotBeNull();
            barracks.Warrior.Should().BeOfType<Ninja>();
            barracks.Weapon.Should().NotBeNull();
            activationCount.Should().Be(1);
        }

        [Fact]
        public void WhenClassHasTwoConstructorsWithInjectAttributeThenAnActivationExceptionIsThrown()
        {
            this.kernel.Bind<ClassWithTwoInjectAttributes>().ToSelf();

            Action getClassWithTwoInjectAttributes = () => this.kernel.Get<ClassWithTwoInjectAttributes>();

            getClassWithTwoInjectAttributes.Should().Throw<ActivationException>();
        }

        [Fact]
        public void WhenConstructorHasSelfBindableTypeItDoesNotCountAsServedParameter()
        {
            var instance = this.kernel.Get<ClassWithSelfBindableType>();

            instance.Sword.Should().BeNull();
        }

        [Fact]
        public void WhenConstructorHasAnOpenGenericTypeItCountsAsServedParameterIfBindingExists()
        {
            this.kernel.Bind(typeof(IGeneric<>)).To(typeof(GenericService<>));
            var instance = this.kernel.Get<ClassWithGeneric>();

            instance.Generic.Should().NotBeNull();
        }

        [Fact]
        public void DoNotChooseObsoleteConstructors()
        {
            this.kernel.Bind<ClassWithObsoleteContructor>().ToSelf();

            var instance = this.kernel.Get<ClassWithObsoleteContructor>();

            instance.Sword.Should().NotBeNull();
        }

        [Fact]
        public void WhenConstructorHasAValueWithDefaultValueItCountsAsServedParameter()
        {
            var instance = this.kernel.Get<ClassWithDefaultValue>();

            instance.X.Should().NotBe(0);
        }

        public class ClassWithDefaultValue
        {
            public ClassWithDefaultValue()
            {
            }

            public ClassWithDefaultValue([DefaultParameterValue(3)] int x)
            {
                this.X = x;
            }

            public int X { get; set; }
        }

        public class ClassWithGeneric
        {
            public ClassWithGeneric()
            {
            }

            public ClassWithGeneric(IGeneric<int> generic)
            {
                this.Generic = generic;
            }

            public IGeneric<int> Generic { get; set; }
        }

        public class ClassWithSelfBindableType
        {
            public ClassWithSelfBindableType()
            {
            }

            public ClassWithSelfBindableType(Sword sword)
            {
                this.Sword = sword;
            }

            public Sword Sword { get; set; }
        }

        public class ClassWithTwoInjectAttributes
        {
            [Inject]
            public ClassWithTwoInjectAttributes()
            {
            }

            [Inject]
            public ClassWithTwoInjectAttributes(int someValue)
            {
            }
        }

        public class ClassWithObsoleteContructor
        {
            [Obsolete]
            public ClassWithObsoleteContructor()
            {
            }

            public ClassWithObsoleteContructor(Sword sword)
            {
                this.Sword = sword;
            }

            public Sword Sword { get; set; }
        }
    }
}