namespace Ninject.Tests.Integration
{
#if !SILVERLIGHT
    using System;

    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;
    
    public class NamedPropertyInjectionTests : IDisposable
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

        public void Dispose()
        {
            this.kernel.Dispose();
        }

        [Fact]
        public void NamedAttributeOfPropertiesAreRespected()
        {
            var ninja = this.kernel.Get<OwnStyleNinja>();

            ninja.MainWeapon.Should().BeOfType<Sword>();
            ninja.OffhandWeapon.Should().BeOfType<ShortSword>();
            ninja.SecretWeaponAccessor.Should().BeOfType<Shuriken>();
            ninja.VerySecretWeaponAccessor.Should().BeOfType<Dagger>();
        }

        [Fact]
        public void NamedAttributeOfPropertiesDefinedOnBaseClassAreRespected()
        {
            var ninja = this.kernel.Get<NinjaWithSpecialMaster>();

            ninja.MainWeapon.Should().BeOfType<Sword>();
            ninja.OffhandWeapon.Should().BeOfType<ShortSword>();
            ninja.SecretWeaponAccessor.Should().BeOfType<Shuriken>();
            ninja.VerySecretWeaponAccessor.Should().BeOfType<Dagger>();
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