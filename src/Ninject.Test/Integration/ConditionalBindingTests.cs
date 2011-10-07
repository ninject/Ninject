namespace Ninject.Tests.Integration
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Ninject.Tests.Integration.StandardKernelTests;
    using Xunit;

    public class ConditionalBindingTests: StandardKernelContext
    {
        [Fact]
        public void GivenADefaultAndSingleSatisfiedConditional_ThenTheConditionalIsUsed()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Samurai>();
            kernel.Bind<Samurai>().ToSelf();
            var warrior = kernel.Get<Samurai>();
            warrior.Weapon.Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void GivenADefaultAndSingleUnatisfiedConditional_ThenTheDefaultIsUsed()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Ninja>();
            kernel.Bind<Samurai>().ToSelf();
            var warrior = kernel.Get<Samurai>();
            warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void GivenADefaultAndAnUnSatisfiedConditional_ThenTheDefaultIsUsed()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Ninja>();
            kernel.Bind<Samurai>().ToSelf();
            var warrior = kernel.Get<Samurai>();
            warrior.Weapon.Should().BeOfType<Sword>();
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
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void GivenBindingIsMadeAfterImplictBinding_ThenExplicitBindingWillResolve()
        {
            IWeapon weapon = kernel.Get<Sword>();
            weapon.Should().BeOfType<Sword>();
            kernel.Bind<Sword>().To<ShortSword>();
            weapon = kernel.Get<Sword>();
            weapon.Should().BeOfType<ShortSword>();
        }

        [Fact]
        public void GivenBothImplicitAndExplicitConditionalBindings_ThenExplicitBindingWillResolve()
        {
            IWeapon weapon = kernel.Get<Sword>();
            // make the binding conditional
            kernel.GetBindings(typeof (Sword)).First().Condition = b => true;
            weapon.Should().BeOfType<Sword>();

            kernel.Bind<Sword>().To<ShortSword>().When(_ => true);
            weapon = kernel.Get<Sword>();
            weapon.Should().BeOfType<ShortSword>();
        }

        [Fact]
        public void GivenADefaultAndAConditionalImplicitBinding_ThenConditionalBindingWillResolve()
        {
            IWeapon weapon = kernel.Get<Sword>();
            // make the binding conditional
            kernel.GetBindings(typeof (Sword)).First().Condition = b => true;
            weapon.Should().BeOfType<Sword>();

            kernel.Bind<Sword>().To<ShortSword>();
            weapon = kernel.Get<Sword>();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void GivenADefaultAndAConditionalBinding_AllBindingsWillResolve()
        {
            var shortSword = new ShortSword();
            var shuriken = new Shuriken();

            kernel.Bind<IWeapon>().ToConstant(shortSword);
            kernel.Bind<IWeapon>().ToConstant(shuriken).When(_ => true);
            var result = kernel.GetAll<IWeapon>();
            result.Should().Contain(shortSword);
            result.Should().Contain(shuriken);
        }

        [Fact]
        public void GivenAMixtureOfBindings_OnlyNonImplicitBindingsWillResolve()
        {
            var shortSword = new ShortSword();
            var sword = new Sword();
            var shuriken = new Shuriken();

            kernel.Bind<IWeapon>().ToConstant(shortSword);
            kernel.Bind<IWeapon>().ToConstant(sword);
            kernel.Bind<IWeapon>().ToConstant(shuriken).Binding.IsImplicit = true;
            var result = kernel.GetAll<IWeapon>();
            result.Should().Contain(shortSword);
            result.Should().Contain(sword);
            result.Should().NotContain(shuriken);
        }

        [Fact]
        public void GivenOnlyImplicitBindings_AllBindingsWillResolve()
        {
            var shortSword = new ShortSword();
            var shuriken = new Shuriken();

            kernel.Bind<IWeapon>().ToConstant(shortSword).Binding.IsImplicit = true;
            kernel.Bind<IWeapon>().ToConstant(shuriken).Binding.IsImplicit = true;
            var result = kernel.GetAll<IWeapon>();
            result.Should().Contain(shortSword);
            result.Should().Contain(shuriken);
        }

        [Fact]
        public void WhenInjectedIntoAppliesToBaseTypes()
        {
            kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<IWeapon>().To<Sword>().WhenInjectedInto<IWarrior>();

            var warrior = kernel.Get<IWarrior>();

            warrior.Weapon.Should().BeOfType<Sword>();
        }
    
        [Fact]
        public void WhenInjectedExactlyIntoAppliesNotToBaseTypes()
        {
            kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<IWeapon>().To<Sword>().WhenInjectedExactlyInto<IWarrior>();

            Action getWarrior = () => kernel.Get<IWarrior>();

            getWarrior.ShouldThrow<ActivationException>();
        }
    
        [Fact]
        public void WhenInjectedExactlyIntoAppliesToServiceType()
        {
            kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<IWeapon>().To<Sword>().WhenInjectedExactlyInto<Samurai>();

            var warrior = kernel.Get<IWarrior>();

            warrior.Weapon.Should().BeOfType<Sword>();
        }
    
        [Fact]
        public void WhenAnyAnchestorNamedAppliesToGrandParentAndParent()
        {
            const string Name = "SomeName";
            kernel.Bind<Barracks>().ToSelf().Named(Name);
            kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<IWeapon>().To<Sword>().WhenAnyAnchestorNamed(Name);

            var barack = kernel.Get<Barracks>();

            barack.Weapon.Should().BeOfType<Sword>();
            barack.Warrior.Weapon.Should().BeOfType<Sword>();
        }
    }
}