using System;
using Ninject.Infrastructure.Disposal;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit
{
    public class PropertyInjectionSelectorContext : DisposableObject
    {
        protected IKernel _kernel;

        protected PropertyInjectionSelectorContext()
        {
            this._kernel = new StandardKernel();
            this._kernel.Bind<IWarrior>()
                    .To<Ninja>();
            this._kernel.Bind<IWeapon>()
                    .To<Sword>();
            this._kernel.Bind<IWeapon>()
                    .To<Shuriken>()
                    .WhenTargetHas<InjectAttribute>();
        }

        public override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this._kernel.Dispose();
                this._kernel = null;
            }
            base.Dispose(disposing);
        }
    }

    public class PropertyInjectionSelectorTests : PropertyInjectionSelectorContext
    {
        [Fact]
        public void NonPublicPropertiesCanBeInjectedWhenEnabled()
        {
            this._kernel.Settings.InjectNonPublic = true;
            var instance = this._kernel.Get<Ninja>();

            Assert.NotNull(instance.Weapon);
            Assert.IsType<Sword>(instance.Weapon);

            Assert.NotNull(instance.SecondaryWeapon);
            Assert.IsType<Shuriken>(instance.SecondaryWeapon);

            Assert.NotNull(instance.VerySecretWeaponAccessor);
            Assert.IsType<Shuriken>(instance.VerySecretWeaponAccessor);
        }

        [Fact]
        public void NonPublicPropertiesCannotBeCreatedByDefault()
        {
            var instance = this._kernel.Get<Ninja>();

            Assert.NotNull(instance.Weapon);
            Assert.Null(instance.SecondaryWeapon);
            Assert.Null(instance.VerySecretWeaponAccessor);
        }
    }
}