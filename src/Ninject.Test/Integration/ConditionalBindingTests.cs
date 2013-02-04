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
            kernel.Bind<IWeapon>().ToConstant(shuriken).BindingConfiguration.IsImplicit = true;
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

            kernel.Bind<IWeapon>().ToConstant(shortSword).BindingConfiguration.IsImplicit = true;
            kernel.Bind<IWeapon>().ToConstant(shuriken).BindingConfiguration.IsImplicit = true;
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
        public void WhenInjectedIntoAppliesToOpenGenerics()
        {
            kernel.Bind(typeof(GenericService<>)).ToSelf();
            kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedInto(typeof(IGenericService<>));
            kernel.Bind<IWeapon>().To<Sword>();

            var service = kernel.Get<GenericService<int>>();

            service.Warrior.Should().BeOfType<Samurai>();
        }

        [Fact]
        public void WhenInjectedIntoAppliesToOpenGenericsWhenClosedGenericIsRequested()
        {
            kernel.Bind(typeof(GenericService<>)).ToSelf();
            kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedInto(typeof(GenericService<>));
            kernel.Bind<IWeapon>().To<Sword>();

            var service = kernel.Get<ClosedGenericService>();

            service.Warrior.Should().BeOfType<Samurai>();
        }
        
        [Fact]
        public void WhenInjectedExactlyIntoAppliesToOpenGenerics()
        {
            kernel.Bind(typeof(GenericService<>)).ToSelf();
            kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedExactlyInto(typeof(GenericService<>));
            kernel.Bind<IWeapon>().To<Sword>();

            var service = kernel.Get<GenericService<int>>();

            service.Warrior.Should().BeOfType<Samurai>();
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
        public void WhenAnyAncestorNamedAppliesToGrandParentAndParent()
        {
            const string Name = "SomeName";
            kernel.Bind<Barracks>().ToSelf().Named(Name);
            kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<IWeapon>().To<Sword>().WhenAnyAncestorNamed(Name);
            kernel.Bind<IWeapon>().To<Dagger>();

            var barack = kernel.Get<Barracks>();

            barack.Weapon.Should().BeOfType<Sword>();
            barack.Warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenNoAncestorNamedAppliesToGrandParentAndParent()
        {
            const string Name = "SomeName";
            kernel.Bind<Barracks>().ToSelf().Named(Name);
            kernel.Bind<IWarrior>().To<Samurai>();

            kernel.Bind<IWeapon>().To<Sword>().WhenNoAncestorNamed(Name);
            kernel.Bind<IWeapon>().To<Dagger>();

            var barack = kernel.Get<Barracks>();

            barack.Weapon.Should().BeOfType<Dagger>();
            barack.Warrior.Weapon.Should().BeOfType<Dagger>();
        }

        [Fact]
        public void WhenAnyAncestorMatchesAppliesToGrandParentAndParent()
        {
            kernel.Bind<Barracks>().ToSelf().WithMetadata("Id", 1);
            kernel.Bind<IWarrior>().To<Samurai>();
            kernel.Bind<IWeapon>().To<Sword>().WhenAnyAncestorMatches(ctx => ctx.Binding.Metadata.Get("Id", -1) == 1);
            kernel.Bind<IWeapon>().To<Dagger>().WhenAnyAncestorMatches(ctx => ctx.Binding.Metadata.Get("Id", -1) == 2);

            var barack = kernel.Get<Barracks>();

            barack.Weapon.Should().BeOfType<Sword>();
            barack.Warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenNoAncestorMatchesAppliesToGrandParentAndParent()
        {
            kernel.Bind<Barracks>().ToSelf().WithMetadata("Id", 1);
            kernel.Bind<IWarrior>().To<Samurai>();

            kernel.Bind<IWeapon>().To<Sword>().WhenNoAncestorMatches(ctx => ctx.Binding.Metadata.Get("Id", -1) == 1);
            kernel.Bind<IWeapon>().To<Dagger>().WhenNoAncestorMatches(ctx => ctx.Binding.Metadata.Get("Id", -1) == 2);

            var barack = kernel.Get<Barracks>();

            barack.Weapon.Should().BeOfType<Dagger>();
            barack.Warrior.Weapon.Should().BeOfType<Dagger>();
        }

        public interface IGenericService<T>
        {
        }

        public class ClosedGenericService : GenericService<int>
        {
            public ClosedGenericService(IWarrior warrior)
                : base(warrior)
            {
            }
        }

        public class GenericService<T> : IGenericService<T>
        {
            public GenericService(IWarrior warrior)
            {
                this.Warrior = warrior;
            }

            public IWarrior Warrior { get; private set; }
        }
    }
}