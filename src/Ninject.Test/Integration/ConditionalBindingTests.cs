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
        public void GivenADefaultAndSingleUnsatisfiedConditional_ThenTheDefaultIsUsed()
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
        public void GivenBindingIsMadeAfterImplicitBinding_ThenExplicitBindingWillResolve()
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
            kernel.Bind<Sword>().ToSelf().When(ctx => true).BindingConfiguration.IsImplicit = true;
            kernel.Bind<Sword>().To<ShortSword>().When(ctx => true);

            var weapon = kernel.Get<Sword>();
            weapon.Should().BeOfType<ShortSword>();
        }

        [Fact]
        public void GivenADefaultAndAConditionalImplicitBinding_ThenConditionalBindingWillResolve()
        {
            kernel.Bind<Sword>().ToSelf().When(ctx => true).BindingConfiguration.IsImplicit = true;
            kernel.Bind<Sword>().To<ShortSword>();

            var weapon = kernel.Get<Sword>();
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
        public void WhenInjectedIntoOneOfMultipleTypesAppliesToOpenGenerics()
        {
            kernel.Bind(typeof(GenericService<>)).ToSelf();
            this.kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedInto(new[] { typeof(IGenericService<>) });
            kernel.Bind<IWeapon>().To<Sword>();

            var service = kernel.Get<GenericService<int>>();
            var anotherService = kernel.Get<AnotherGenericService<int>>();

            service.Warrior.Should().BeOfType<Samurai>();
            anotherService.Warrior.Should().BeOfType<Samurai>();
        }

        [Fact]
        public void WhenInjectedIntoAppliesToOneOfMultipleServiceType()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWarrior>().To<FootSoldier>();
            kernel.Bind<IWeapon>().To<Shuriken>()
                .WhenInjectedExactlyInto(typeof(Samurai), typeof(Barracks));

            kernel.Bind<Samurai>().ToSelf();
            kernel.Bind<Barracks>().ToSelf();
            kernel.Bind<NinjaBarracks>().ToSelf();

            var warrior = kernel.Get<Samurai>();
            var barracks = kernel.Get<Barracks>();
            var ninja = kernel.Get<NinjaBarracks>();

            warrior.Weapon.Should().BeOfType<Shuriken>();
            barracks.Weapon.Should().BeOfType<Shuriken>();
            ninja.Weapon.Should().BeOfType<Sword>();
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
        public void WhenInjectedIntoOneOfMultipleTypesAppliesToOpenGenericsWhenClosedGenericIsRequested()
        {
            kernel.Bind(typeof(GenericService<>)).ToSelf();
            kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedInto(typeof(GenericService<>), typeof(AnotherGenericService<>));
            kernel.Bind<IWeapon>().To<Sword>();

            var service = kernel.Get<ClosedGenericService>();
            var anotherService = kernel.Get<ClosedAnotherGenericService>();

            service.Warrior.Should().BeOfType<Samurai>();
            anotherService.Warrior.Should().BeOfType<Samurai>();
        }

        [Fact]
        public void WhenInjectedIntoOneOfMultipleDoesNotApplyForConcreteTypes()
        {

            kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto(new[] { typeof(Samurai) });
            kernel.Bind<Samurai>().ToSelf();
            var warrior = kernel.Get<Samurai>();
            warrior.Weapon.Should().BeOfType<Shuriken>();
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
        public void WhenInjectedExactlyIntoOneOfMultipleTypesAppliesToOpenGenerics()
        {
            kernel.Bind(typeof(GenericService<>)).ToSelf();
            kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedExactlyInto(typeof(GenericService<>), typeof(AnotherGenericService<>));
            kernel.Bind<IWeapon>().To<Sword>();

            var service = kernel.Get<GenericService<int>>();
            var anotherService = kernel.Get<AnotherGenericService<int>>();

            service.Warrior.Should().BeOfType<Samurai>();
            anotherService.Warrior.Should().BeOfType<Samurai>();
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
        public void WhenInjectedExactlyIntoAppliesToOneOfMultipleServiceType()
        {
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWarrior>().To<FootSoldier>();
            kernel.Bind<IWeapon>().To<Shuriken>()
                .WhenInjectedExactlyInto(typeof(Samurai), typeof(Barracks));

            kernel.Bind<Samurai>().ToSelf();
            kernel.Bind<Barracks>().ToSelf();
            kernel.Bind<NinjaBarracks>().ToSelf();

            var warrior = kernel.Get<Samurai>();
            var barracks = kernel.Get<Barracks>();
            var ninja = kernel.Get<NinjaBarracks>();

            warrior.Weapon.Should().BeOfType<Shuriken>();
            barracks.Weapon.Should().BeOfType<Shuriken>();
            ninja.Weapon.Should().BeOfType<Sword>();
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

        [Fact]
        public void WhenMemberHasDoesNotConsiderAttributeOnTarget()
        {
            kernel.Bind<Knight>().ToSelf();
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<ShortSword>().WhenMemberHas<WeakAttribute>();

            var knight = kernel.Get<Knight>();
            knight.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenMemberHasDoesConsiderAttributeOnMember()
        {
            kernel.Bind<Knight>().ToSelf();
            kernel.Bind<IWeapon>().To<Sword>().WhenMemberHas<StrongAttribute>();
            kernel.Bind<IWeapon>().To<ShortSword>();

            var knight = kernel.Get<Knight>();
            knight.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenTargetHasDoesConsiderAttributeOnTarget()
        {
            kernel.Bind<Knight>().ToSelf();
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<IWeapon>().To<ShortSword>().WhenTargetHas<WeakAttribute>();

            var knight = kernel.Get<Knight>();
            knight.Weapon.Should().BeOfType<ShortSword>();
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

        public class ClosedAnotherGenericService : AnotherGenericService<int>
        {
            public ClosedAnotherGenericService(IWarrior warrior)
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

        public class AnotherGenericService<T> : IGenericService<T>
        {
            public AnotherGenericService(IWarrior warrior)
            {
                this.Warrior = warrior;
            }

            public IWarrior Warrior { get; protected set; }
        }

        public class Knight
        {
            public IWeapon Weapon { get; private set; }

            [Strong]
            public Knight([Weak] IWeapon weapon)
            {
                this.Weapon = weapon;
            }
        }
    }
}