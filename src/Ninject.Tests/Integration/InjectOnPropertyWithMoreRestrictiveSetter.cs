namespace Ninject.Tests.Integration
{
    using Fakes;
    using StandardKernelTests;
    using Xunit;
    using Xunit.Should;

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

            warrior.ShouldNotBeNull();
            warrior.Weapon.ShouldNotBeNull();
            warrior.Weapon.ShouldBeInstanceOf(typeof(Shuriken));
            warrior.SecretWeapon.ShouldNotBeNull();
            warrior.SecretWeapon.ShouldBeInstanceOf(typeof(Sword));
            warrior.UltraSecretWeapon.ShouldNotBeNull();
            warrior.UltraSecretWeapon.ShouldBeInstanceOf(typeof(ShortSword));
        }
#endif //!SILVERLIGHT

        [Fact]
        public void NonPublicPropertiesWithMoreRestrictiveSetterCannotBeCreatedByDefault()
        {
            var warrior = this.kernel.Get<SpecialNinja>();

            warrior.ShouldNotBeNull();
            warrior.Weapon.ShouldBeNull();
            warrior.SecretWeapon.ShouldBeNull();
            warrior.UltraSecretWeapon.ShouldBeNull();
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

            warrior.ShouldNotBeNull();
            warrior.Weapon.ShouldNotBeNull();
            warrior.Weapon.ShouldBeInstanceOf(typeof(Shuriken));
            warrior.SecretWeapon.ShouldNotBeNull();
            warrior.SecretWeapon.ShouldBeInstanceOf(typeof(Sword));
            warrior.UltraSecretWeapon.ShouldBeNull();
        }
#endif //!SILVERLIGHT

        [Fact]
        public void NonPublicPropertiesWithMoreRestrictiveSetterInHierarchyCannotBeCreatedByDefault()
        {
            var warrior = this.kernel.Get<UltraSpecialNinja>();

            warrior.ShouldNotBeNull();
            warrior.Weapon.ShouldBeNull();
            warrior.SecretWeapon.ShouldBeNull();
            warrior.UltraSecretWeapon.ShouldBeNull();
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
    }

    public class UltraSpecialNinja : SpecialNinja
    {
    }
}