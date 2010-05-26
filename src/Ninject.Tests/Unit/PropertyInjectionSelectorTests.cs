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
			_kernel = new StandardKernel();
			_kernel.Bind<IWarrior>()
					.To<Ninja>();
			_kernel.Bind<IWeapon>()
					.To<Sword>();
			_kernel.Bind<IWeapon>()
					.To<Shuriken>()
					.WhenTargetHas<InjectAttribute>();
		}

		public override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed)
			{
				_kernel.Dispose();
				_kernel = null;
			}
			base.Dispose(disposing);
		}
	}

	public class PropertyInjectionSelectorTests : PropertyInjectionSelectorContext
	{
#if !SILVERLIGHT
		[Fact]
		public void NonPublicPropertiesCanBeInjectedWhenEnabled()
		{
			_kernel.Settings.InjectNonPublic = true;
			var instance = _kernel.Get<Ninja>();

			Assert.NotNull(instance.Weapon);
			Assert.IsType<Sword>(instance.Weapon);

			Assert.NotNull(instance.SecondaryWeapon);
			Assert.IsType<Shuriken>(instance.SecondaryWeapon);

			Assert.NotNull(instance.SecretWeaponAccessor);
			Assert.IsType<Shuriken>(instance.SecretWeaponAccessor);
		}
#endif //!SILVERLIGHT

		[Fact]
		public void NonPublicPropertiesCannotBeCreatedByDefault()
		{
			var instance = _kernel.Get<Ninja>();

			Assert.NotNull(instance.Weapon);
			Assert.Null(instance.SecondaryWeapon);
			Assert.Null(instance.SecretWeaponAccessor);
		}
	}
}