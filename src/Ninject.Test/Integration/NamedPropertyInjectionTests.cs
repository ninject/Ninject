namespace Ninject.Tests.Integration
{
    using System;

    using FluentAssertions;
    using Ninject.Tests.Fakes;
    using Xunit;
    
    public class NamedPropertyInjectionTests : IDisposable
    {
        private readonly IKernel kernel;

        public NamedPropertyInjectionTests()
        {
            this.kernel = new StandardKernel();
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
            ninja.SecretWeapon.Should().BeOfType<Shuriken>();
            ninja.VerySecretWeapon.Should().BeOfType<Dagger>();
        }

        [Fact]
        public void NamedAttributeOfPropertiesDefinedOnBaseClassAreRespected()
        {
            var ninja = this.kernel.Get<NinjaWithSpecialMaster>();

            ninja.MainWeapon.Should().BeOfType<Sword>();
            ninja.OffhandWeapon.Should().BeOfType<ShortSword>();
            ninja.SecretWeapon.Should().BeOfType<Shuriken>();
            ninja.VerySecretWeapon.Should().BeOfType<Dagger>();
        }
        
        public class OwnStyleNinja
        {
            [Inject]
            [Named("Main")]
            public virtual IWeapon MainWeapon { get; set; }
            
            [Inject]
            [Named("Offhand")]
            public virtual IWeapon OffhandWeapon { get; set; }

            [Inject]
            [Named("Secret")]
            public virtual IWeapon SecretWeapon { get; set; }

            [Inject]
            [Named("VerySecret")]
            public IWeapon VerySecretWeapon { get; set; }
        }

        public class NinjaWithSpecialMaster : OwnStyleNinja
        {
            public override IWeapon MainWeapon { get; set; }

            public override IWeapon OffhandWeapon { get; set; }

            public override IWeapon SecretWeapon { get; set; }
        }
    }
}