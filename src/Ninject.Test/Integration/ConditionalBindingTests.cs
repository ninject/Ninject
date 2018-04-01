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
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Samurai>();
            this.kernel.Bind<Samurai>().ToSelf();
            var warrior = this.kernel.Get<Samurai>();
            warrior.Weapon.Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void GivenADefaultAndSingleUnsatisfiedConditional_ThenTheDefaultIsUsed()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Ninja>();
            this.kernel.Bind<Samurai>().ToSelf();
            var warrior = this.kernel.Get<Samurai>();
            warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void GivenADefaultAndAnUnSatisfiedConditional_ThenTheDefaultIsUsed()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Ninja>();
            this.kernel.Bind<Samurai>().ToSelf();
            var warrior = this.kernel.Get<Samurai>();
            warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void GivenADefaultAndAnManySatisfiedConditionals_ThenAnExceptionIsThrown()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Sword>().WhenInjectedInto<Samurai>();
            this.kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto<Samurai>();
            this.kernel.Bind<Samurai>().ToSelf();
            Assert.Throws<ActivationException>(() => this.kernel.Get<Samurai>());
        }

        [Fact]
        public void GivenNoBinding_ThenASelfBindableTypeWillResolve()
        {
            var weapon = this.kernel.Get<Sword>();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void GivenBindingIsMadeAfterImplicitBinding_ThenExplicitBindingWillResolve()
        {
            IWeapon weapon = this.kernel.Get<Sword>();
            weapon.Should().BeOfType<Sword>();
            this.kernel.Bind<Sword>().To<ShortSword>();
            weapon = this.kernel.Get<Sword>();
            weapon.Should().BeOfType<ShortSword>();
        }

        [Fact]
        public void GivenBothImplicitAndExplicitConditionalBindings_ThenExplicitBindingWillResolve()
        {
            this.kernel.Bind<Sword>().ToSelf().When(ctx => true).BindingConfiguration.IsImplicit = true;
            this.kernel.Bind<Sword>().To<ShortSword>().When(ctx => true);

            var weapon = this.kernel.Get<Sword>();
            weapon.Should().BeOfType<ShortSword>();
        }

        [Fact]
        public void GivenADefaultAndAConditionalImplicitBinding_ThenConditionalBindingWillResolve()
        {
            this.kernel.Bind<Sword>().ToSelf().When(ctx => true).BindingConfiguration.IsImplicit = true;
            this.kernel.Bind<Sword>().To<ShortSword>();

            var weapon = this.kernel.Get<Sword>();
            weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void GivenADefaultAndAConditionalBinding_AllBindingsWillResolve()
        {
            var shortSword = new ShortSword();
            var shuriken = new Shuriken();

            this.kernel.Bind<IWeapon>().ToConstant(shortSword);
            this.kernel.Bind<IWeapon>().ToConstant(shuriken).When(_ => true);
            var result = this.kernel.GetAll<IWeapon>();
            result.Should().Contain(shortSword);
            result.Should().Contain(shuriken);
        }

        [Fact]
        public void GivenAMixtureOfBindings_OnlyNonImplicitBindingsWillResolve()
        {
            var shortSword = new ShortSword();
            var sword = new Sword();
            var shuriken = new Shuriken();

            this.kernel.Bind<IWeapon>().ToConstant(shortSword);
            this.kernel.Bind<IWeapon>().ToConstant(sword);
            this.kernel.Bind<IWeapon>().ToConstant(shuriken).BindingConfiguration.IsImplicit = true;
            var result = this.kernel.GetAll<IWeapon>();
            result.Should().Contain(shortSword);
            result.Should().Contain(sword);
            result.Should().NotContain(shuriken);
        }

        [Fact]
        public void GivenOnlyImplicitBindings_AllBindingsWillResolve()
        {
            var shortSword = new ShortSword();
            var shuriken = new Shuriken();

            this.kernel.Bind<IWeapon>().ToConstant(shortSword).BindingConfiguration.IsImplicit = true;
            this.kernel.Bind<IWeapon>().ToConstant(shuriken).BindingConfiguration.IsImplicit = true;
            var result = this.kernel.GetAll<IWeapon>();
            result.Should().Contain(shortSword);
            result.Should().Contain(shuriken);
        }

        [Fact]
        public void WhenInjectedIntoAppliesToBaseTypes()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().WhenInjectedInto<IWarrior>();

            var warrior = this.kernel.Get<IWarrior>();

            warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenInjectedIntoAppliesToOpenGenerics()
        {
            this.kernel.Bind(typeof(GenericService<>)).ToSelf();
            this.kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedInto(typeof(IGenericService<>));
            this.kernel.Bind<IWeapon>().To<Sword>();

            var service = this.kernel.Get<GenericService<int>>();

            service.Warrior.Should().BeOfType<Samurai>();
        }

        [Fact]
        public void WhenInjectedIntoOneOfMultipleTypesAppliesToOpenGenerics()
        {
            this.kernel.Bind(typeof(GenericService<>)).ToSelf();
            this.kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedInto(new[] { typeof(IGenericService<>) });
            this.kernel.Bind<IWeapon>().To<Sword>();

            var service = this.kernel.Get<GenericService<int>>();
            var anotherService = this.kernel.Get<AnotherGenericService<int>>();

            service.Warrior.Should().BeOfType<Samurai>();
            anotherService.Warrior.Should().BeOfType<Samurai>();
        }

        [Fact]
        public void WhenInjectedIntoAppliesToOneOfMultipleServiceType()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWarrior>().To<FootSoldier>();
            this.kernel.Bind<IWeapon>().To<Shuriken>()
                .WhenInjectedExactlyInto(typeof(Samurai), typeof(Barracks));

            this.kernel.Bind<Samurai>().ToSelf();
            this.kernel.Bind<Barracks>().ToSelf();
            this.kernel.Bind<NinjaBarracks>().ToSelf();

            var warrior = this.kernel.Get<Samurai>();
            var barracks = this.kernel.Get<Barracks>();
            var ninja = this.kernel.Get<NinjaBarracks>();

            warrior.Weapon.Should().BeOfType<Shuriken>();
            barracks.Weapon.Should().BeOfType<Shuriken>();
            ninja.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenInjectedIntoAppliesToOpenGenericsWhenClosedGenericIsRequested()
        {
            this.kernel.Bind(typeof(GenericService<>)).ToSelf();
            this.kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedInto(typeof(GenericService<>));
            this.kernel.Bind<IWeapon>().To<Sword>();

            var service = this.kernel.Get<ClosedGenericService>();

            service.Warrior.Should().BeOfType<Samurai>();
        }

        [Fact]
        public void WhenInjectedIntoOneOfMultipleTypesAppliesToOpenGenericsWhenClosedGenericIsRequested()
        {
            this.kernel.Bind(typeof(GenericService<>)).ToSelf();
            this.kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedInto(typeof(GenericService<>), typeof(AnotherGenericService<>));
            this.kernel.Bind<IWeapon>().To<Sword>();

            var service = this.kernel.Get<ClosedGenericService>();
            var anotherService = this.kernel.Get<ClosedAnotherGenericService>();

            service.Warrior.Should().BeOfType<Samurai>();
            anotherService.Warrior.Should().BeOfType<Samurai>();
        }

        [Fact]
        public void WhenInjectedIntoOneOfMultipleDoesNotApplyForConcreteTypes()
        {

            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedInto(new[] { typeof(Samurai) });
            this.kernel.Bind<Samurai>().ToSelf();
            var warrior = this.kernel.Get<Samurai>();
            warrior.Weapon.Should().BeOfType<Shuriken>();
        }

        [Fact]
        public void WhenInjectedExactlyIntoAppliesToOpenGenerics()
        {
            this.kernel.Bind(typeof(GenericService<>)).ToSelf();
            this.kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedExactlyInto(typeof(GenericService<>));
            this.kernel.Bind<IWeapon>().To<Sword>();

            var service = this.kernel.Get<GenericService<int>>();

            service.Warrior.Should().BeOfType<Samurai>();
        }

        [Fact]
        public void WhenInjectedExactlyIntoOneOfMultipleTypesAppliesToOpenGenerics()
        {
            this.kernel.Bind(typeof(GenericService<>)).ToSelf();
            this.kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedExactlyInto(typeof(GenericService<>), typeof(AnotherGenericService<>));
            this.kernel.Bind<IWeapon>().To<Sword>();

            var service = this.kernel.Get<GenericService<int>>();
            var anotherService = this.kernel.Get<AnotherGenericService<int>>();

            service.Warrior.Should().BeOfType<Samurai>();
            anotherService.Warrior.Should().BeOfType<Samurai>();
        }

        [Fact]
        public void WhenInjectedExactlyIntoAppliesNotToBaseTypes()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().WhenInjectedExactlyInto<IWarrior>();

            Action getWarrior = () => this.kernel.Get<IWarrior>();

            getWarrior.Should().Throw<ActivationException>();
        }
    
        [Fact]
        public void WhenInjectedExactlyIntoAppliesToServiceType()
        {
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().WhenInjectedExactlyInto<Samurai>();

            var warrior = this.kernel.Get<IWarrior>();

            warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenInjectedExactlyIntoAppliesToOneOfMultipleServiceType()
        {
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWarrior>().To<FootSoldier>();
            this.kernel.Bind<IWeapon>().To<Shuriken>()
                .WhenInjectedExactlyInto(typeof(Samurai), typeof(Barracks));

            this.kernel.Bind<Samurai>().ToSelf();
            this.kernel.Bind<Barracks>().ToSelf();
            this.kernel.Bind<NinjaBarracks>().ToSelf();

            var warrior = this.kernel.Get<Samurai>();
            var barracks = this.kernel.Get<Barracks>();
            var ninja = this.kernel.Get<NinjaBarracks>();

            warrior.Weapon.Should().BeOfType<Shuriken>();
            barracks.Weapon.Should().BeOfType<Shuriken>();
            ninja.Weapon.Should().BeOfType<Sword>();
        }
    
        [Fact]
        public void WhenAnyAncestorNamedAppliesToGrandParentAndParent()
        {
            const string Name = "SomeName";
            this.kernel.Bind<Barracks>().ToSelf().Named(Name);
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().WhenAnyAncestorNamed(Name);
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var barack = this.kernel.Get<Barracks>();

            barack.Weapon.Should().BeOfType<Sword>();
            barack.Warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenNoAncestorNamedAppliesToGrandParentAndParent()
        {
            const string Name = "SomeName";
            this.kernel.Bind<Barracks>().ToSelf().Named(Name);
            this.kernel.Bind<IWarrior>().To<Samurai>();

            this.kernel.Bind<IWeapon>().To<Sword>().WhenNoAncestorNamed(Name);
            this.kernel.Bind<IWeapon>().To<Dagger>();

            var barack = this.kernel.Get<Barracks>();

            barack.Weapon.Should().BeOfType<Dagger>();
            barack.Warrior.Weapon.Should().BeOfType<Dagger>();
        }

        [Fact]
        public void WhenAnyAncestorMatchesAppliesToGrandParentAndParent()
        {
            this.kernel.Bind<Barracks>().ToSelf().WithMetadata("Id", 1);
            this.kernel.Bind<IWarrior>().To<Samurai>();
            this.kernel.Bind<IWeapon>().To<Sword>().WhenAnyAncestorMatches(ctx => ctx.Binding.Metadata.Get("Id", -1) == 1);
            this.kernel.Bind<IWeapon>().To<Dagger>().WhenAnyAncestorMatches(ctx => ctx.Binding.Metadata.Get("Id", -1) == 2);

            var barack = this.kernel.Get<Barracks>();

            barack.Weapon.Should().BeOfType<Sword>();
            barack.Warrior.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenNoAncestorMatchesAppliesToGrandParentAndParent()
        {
            this.kernel.Bind<Barracks>().ToSelf().WithMetadata("Id", 1);
            this.kernel.Bind<IWarrior>().To<Samurai>();

            this.kernel.Bind<IWeapon>().To<Sword>().WhenNoAncestorMatches(ctx => ctx.Binding.Metadata.Get("Id", -1) == 1);
            this.kernel.Bind<IWeapon>().To<Dagger>().WhenNoAncestorMatches(ctx => ctx.Binding.Metadata.Get("Id", -1) == 2);

            var barack = this.kernel.Get<Barracks>();

            barack.Weapon.Should().BeOfType<Dagger>();
            barack.Warrior.Weapon.Should().BeOfType<Dagger>();
        }

        [Fact]
        public void WhenMemberHasDoesNotConsiderAttributeOnTarget()
        {
            this.kernel.Bind<Knight>().ToSelf();
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<ShortSword>().WhenMemberHas<WeakAttribute>();

            var knight = this.kernel.Get<Knight>();
            knight.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenMemberHasDoesConsiderAttributeOnMember()
        {
            this.kernel.Bind<Knight>().ToSelf();
            this.kernel.Bind<IWeapon>().To<Sword>().WhenMemberHas<StrongAttribute>();
            this.kernel.Bind<IWeapon>().To<ShortSword>();

            var knight = this.kernel.Get<Knight>();
            knight.Weapon.Should().BeOfType<Sword>();
        }

        [Fact]
        public void WhenTargetHasDoesConsiderAttributeOnTarget()
        {
            this.kernel.Bind<Knight>().ToSelf();
            this.kernel.Bind<IWeapon>().To<Sword>();
            this.kernel.Bind<IWeapon>().To<ShortSword>().WhenTargetHas<WeakAttribute>();

            var knight = this.kernel.Get<Knight>();
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