namespace Ninject.Tests.Integration
{
    using Ninject.Tests.Fakes;
    using Xunit;
    using Xunit.Should;


#if !SILVERLIGHT
    public class NamedPropertyInjectionTests
    {
        private readonly IKernel kernel;

        public NamedPropertyInjectionTests()
        {
            this.kernel = new StandardKernel(new NinjectSettings() { InjectNonPublic = true, InjectParentPrivateProperties = true });
            this.kernel.Bind<IWeapon>().To<Sword>().Named("Main");
            this.kernel.Bind<IWeapon>().To<ShortSword>().Named("Offhand");
            this.kernel.Bind<IWeapon>().To<Shuriken>().Named("Secret");
            this.kernel.Bind<IWeapon>().To<Dagger>().Named("VerySecret");
        }

        [Fact]
        public void NamedAttributeOfPropertiesAreRespected()
        {
            var ninja = this.kernel.Get<OwnStyleNinja>();

            ninja.MainWeapon.ShouldBeInstanceOf<Sword>();
            ninja.OffhandWeapon.ShouldBeInstanceOf<ShortSword>();
            ninja.SecretWeaponAccessor.ShouldBeInstanceOf<Shuriken>();
            ninja.VerySecretWeaponAccessor.ShouldBeInstanceOf<Dagger>();
        }

        [Fact]
        public void NamedAttributeOfPropertiesDefinedOnBaseClassAreRespected()
        {
            var ninja = this.kernel.Get<NinjaWithSpecialMaster>();

            ninja.MainWeapon.ShouldBeInstanceOf<Sword>();
            ninja.OffhandWeapon.ShouldBeInstanceOf<ShortSword>();
            ninja.SecretWeaponAccessor.ShouldBeInstanceOf<Shuriken>();
            ninja.VerySecretWeaponAccessor.ShouldBeInstanceOf<Dagger>();
        }
        
        public class OwnStyleNinja
        {
            [Inject]
            [Named("Main")]
            public virtual IWeapon MainWeapon { get; set; }

            public IWeapon SecretWeaponAccessor
            {
                get
                {
                    return this.SecretWeapon;
                }
            }

            public IWeapon VerySecretWeaponAccessor
            {
                get
                {
                    return this.VerySecretWeapon;
                }
            }
            
            [Inject]
            [Named("Offhand")]
            internal virtual IWeapon OffhandWeapon { get; set; }

            [Inject]
            [Named("Secret")]
            protected virtual IWeapon SecretWeapon { get; set; }

            [Inject]
            [Named("VerySecret")]
            private IWeapon VerySecretWeapon { get; set; }
        }

        public class NinjaWithSpecialMaster : OwnStyleNinja
        {
            public override IWeapon MainWeapon { get; set; }

            internal override IWeapon OffhandWeapon { get; set; }

            protected override IWeapon SecretWeapon { get; set; }
        }
    }
#endif
}