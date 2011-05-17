namespace Ninject.Tests.Integration
{
    using Fakes;
    using FluentAssertions;
    using StandardKernelTests;
    using Xunit;

    public class PublicPropertyWithMoreRestrictiveSetterContext : StandardKernelContext
    {
        public PublicPropertyWithMoreRestrictiveSetterContext()
        {
            this.kernel.Bind<IWarrior>().To<SpecialNinja>();
            this.kernel.Bind<UltraSpecialNinja>().ToSelf();
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("Weapon");
            this.kernel.Bind<IWeapon>().To<Sword>().Named("SecretWeapon");
            this.kernel.Bind<IWeapon>().To<ShortSword>().Named("UltraSecretWeapon");
        }
    }

    public class WhenInjectOnPublicPropertyWithMoreRestrictiveSetter : PublicPropertyWithMoreRestrictiveSetterContext
    {
#if !SILVERLIGHT
        [Fact]
        public void NonPublicPropertiesWithMoreRestrictiveSetterCanBeInjectedWhenEnabled()
        {
            this.kernel.Settings.InjectNonPublic = true;
            var warrior = this.kernel.Get<SpecialNinja>();

            warrior.Should().NotBeNull();
            warrior.Weapon.Should().NotBeNull();
            warrior.Weapon.Should().BeOfType<Shuriken>();
            warrior.SecretWeapon.Should().NotBeNull();
            warrior.SecretWeapon.Should().BeOfType<Sword>();
            warrior.UltraSecretWeapon.Should().NotBeNull();
            warrior.UltraSecretWeapon.Should().BeOfType<ShortSword>();
        }
#endif //!SILVERLIGHT

        [Fact]
        public void NonPublicPropertiesWithMoreRestrictiveSetterCannotBeCreatedByDefault()
        {
            var warrior = this.kernel.Get<SpecialNinja>();

            warrior.Should().NotBeNull();
            warrior.Weapon.Should().BeNull();
            warrior.SecretWeapon.Should().BeNull();
            warrior.UltraSecretWeapon.Should().BeNull();
        }
    }

    public class WhenInjectOnPublicPropertyWithMoreRestrictiveSetterInHierarchy : PublicPropertyWithMoreRestrictiveSetterContext
    {
#if !SILVERLIGHT
        [Fact]
        public void NonPublicPropertiesWithMoreRestrictiveSetterInHierarchyExceptPrivateCanBeInjectedWhenEnabled()
        {
            this.kernel.Settings.InjectNonPublic = true;
            var warrior = this.kernel.Get<UltraSpecialNinja>();

            warrior.Should().NotBeNull();
            warrior.Weapon.Should().NotBeNull();
            warrior.Weapon.Should().BeOfType<Shuriken>();
            warrior.SecretWeapon.Should().NotBeNull();
            warrior.SecretWeapon.Should().BeOfType<Sword>();
            warrior.UltraSecretWeapon.Should().NotBeNull();
            warrior.UltraSecretWeapon.Should().BeOfType<ShortSword>();
        }
#endif //!SILVERLIGHT

        [Fact]
        public void NonPublicPropertiesWithMoreRestrictiveSetterInHierarchyCannotBeCreatedByDefault()
        {
            var warrior = this.kernel.Get<UltraSpecialNinja>();

            warrior.Should().NotBeNull();
            warrior.Weapon.Should().BeNull();
            warrior.SecretWeapon.Should().BeNull();
            warrior.UltraSecretWeapon.Should().BeNull();
        }
    }

    public class SpecialNinja : IWarrior
    {
        [Inject]
        [Named("Weapon")]
        public IWeapon Weapon { get; internal set; }

        [Inject]
        [Named("SecretWeapon")]
        public IWeapon SecretWeapon { get; protected set; }

        [Inject]
        [Named("UltraSecretWeapon")]
        public IWeapon UltraSecretWeapon { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// Added to have properties with the same name.
        /// </summary>
        /// <value>Allways null.</value>
        public object this[int index]
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// Added to have properties with the same name.
        /// </summary>
        /// <value>Allways null.</value>
        public object this[string index]
        {
            get { return null; }
            set { }
        }
    }

    public class UltraSpecialNinja : SpecialNinja
    {
    }
}