namespace Ninject.Tests.Integration
{
    using FluentAssertions;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Parameters;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class WithPropertyValueTests : PropertyInjectionTests
    {
        [Fact]
        public void PropertyValueIsAssignedWhenNoInjectAttributeIsSuppliedUsingCallback()
        {
            this.kernel.Bind<IWarrior>().To<FootSoldier>()
                .WithPropertyValue("Weapon", context => context.Kernel.Get<IWeapon>());
            var warrior = this.kernel.Get<IWarrior>();
            ValidateWarrior(warrior);
        }

        [Fact]
        public void PropertyValueIsAssignedWhenNoInjectAttributeUsingSuppliedValue()
        {
            this.kernel.Bind<IWarrior>().To<FootSoldier>()
                .WithPropertyValue("Weapon", this.kernel.Get<IWeapon>());
            var warrior = this.kernel.Get<IWarrior>();
            ValidateWarrior(warrior);
        }

#if !SILVERLIGHT
        [Fact]
        public void PropertyValuesOverrideDefaultBinding()
        {
            this.kernel.Settings.InjectNonPublic = true;
            this.kernel.Settings.InjectParentPrivateProperties = true;
            this.kernel.Bind<IWarrior>().To<Ninja>()
                .WithPropertyValue("SecondaryWeapon", context => new Sword())
                .WithPropertyValue("VerySecretWeapon", context => new Sword());
            var warrior = this.kernel.Get<IWarrior>();
            ValidateNinjaWarriorWithOverides(warrior);
        }
#endif //!SILVERLIGHT
    }

    public class WithParameterTests : PropertyInjectionTests
    {
        [Fact]
        public void PropertyValueIsAssignedWhenNoInjectAttributeIsSuppliedUsingCallback()
        {
            this.kernel.Bind<IWarrior>().To<FootSoldier>()
                .WithParameter(new PropertyValue("Weapon", context => context.Kernel.Get<IWeapon>()));
            var warrior = this.kernel.Get<IWarrior>();
            ValidateWarrior(warrior);
        }

        [Fact]
        public void PropertyValueIsAssignedWhenNoInjectAttributeUsingSuppliedValue()
        {
            this.kernel.Bind<IWarrior>().To<FootSoldier>()
                .WithParameter(new PropertyValue("Weapon", this.kernel.Get<IWeapon>()));
            var warrior = this.kernel.Get<IWarrior>();
            ValidateWarrior(warrior);
        }

#if !SILVERLIGHT
        [Fact]
        public void PropertyValuesOverrideDefaultBinding()
        {
            this.kernel.Settings.InjectNonPublic = true;
            this.kernel.Settings.InjectParentPrivateProperties = true;
            this.kernel.Bind<IWarrior>().To<Ninja>()
                .WithParameter(new PropertyValue("SecondaryWeapon", context => new Sword()))
                .WithParameter(new PropertyValue("VerySecretWeapon", context => new Sword()));
            var warrior = this.kernel.Get<IWarrior>();
            ValidateNinjaWarriorWithOverides(warrior);
        }
#endif //!SILVERLIGHT
    }

    public class WhenNoPropertyOverridesAreSupplied : PropertyInjectionTests
    {
#if !SILVERLIGHT
        [Fact]
        public void DefaultBindingsAreUsed()
        {
            this.kernel.Settings.InjectNonPublic = true;
            this.kernel.Bind<IWarrior>().To<Ninja>();
            var warrior = this.kernel.Get<IWarrior>();
            Assert.IsType<Ninja>(warrior);
            Assert.IsType<Shuriken>(warrior.Weapon);
            Ninja ninja = warrior as Ninja;
            Assert.IsType<Shuriken>(ninja.SecondaryWeapon);
            Assert.IsType<Shuriken>(ninja.VerySecretWeaponAccessor);
        }

        [Fact]
        public void OverriddenPropertiesAreInjected()
        {
            this.kernel.Settings.InjectNonPublic = true;
            this.kernel.Settings.InjectParentPrivateProperties = true;
            var warrior = this.kernel.Get<OwnStyleNinja>();

            warrior.Should().NotBeNull();
            warrior.OffHandWeapon.Should().NotBeNull();
            warrior.SecondaryWeapon.Should().NotBeNull();
            warrior.SecretWeaponAccessor.Should().NotBeNull();
            warrior.VerySecretWeaponAccessor.Should().NotBeNull();
        }

        [Fact]
        public void ParentPropertiesAreInjected()
        {
            this.kernel.Settings.InjectNonPublic = true;
            this.kernel.Settings.InjectParentPrivateProperties = true;
            var warrior = this.kernel.Get<FatherStyleNinja>();

            warrior.Should().NotBeNull();
            warrior.OffHandWeapon.Should().NotBeNull();
            warrior.SecondaryWeapon.Should().NotBeNull();
            warrior.SecretWeaponAccessor.Should().NotBeNull();
            warrior.VerySecretWeaponAccessor.Should().NotBeNull();
        }
        
        private class OwnStyleNinja : Ninja
        {
            public OwnStyleNinja(IWeapon weapon)
                : base(weapon)
            {
            }

            public override IWeapon OffHandWeapon { get; set; }

            internal override IWeapon SecondaryWeapon { get; set; }

            protected override IWeapon SecretWeapon { get; set; }
        }

        private class FatherStyleNinja : Ninja
        {
            public FatherStyleNinja(IWeapon weapon)
                : base(weapon)
            {
            }
        }
#endif //!SILVERLIGHT
    }

    public abstract class PropertyInjectionTests : DisposableObject
    {
        protected IKernel kernel;

        public PropertyInjectionTests()
        {
            this.kernel = new StandardKernel();
            this.kernel.Bind<IWeapon>().To<Shuriken>();
        }

        protected void ValidateWarrior(IWarrior warrior)
        {
            warrior.Should().BeOfType<FootSoldier>();
            warrior.Weapon.Should().NotBeNull();
            warrior.Weapon.Should().BeOfType<Shuriken>();
        }

        protected void ValidateNinjaWarriorWithOverides(IWarrior warrior)
        {
            warrior.Should().BeOfType<Ninja>();
            warrior.Weapon.Should().BeOfType<Shuriken>();
            Ninja ninja = warrior as Ninja;
            ninja.SecondaryWeapon.Should().BeOfType<Sword>();
            ninja.VerySecretWeaponAccessor.Should().BeOfType<Sword>();
        }

        public override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                this.kernel.Dispose();
                this.kernel = null;
            }
            base.Dispose(disposing);
        }
    }
}