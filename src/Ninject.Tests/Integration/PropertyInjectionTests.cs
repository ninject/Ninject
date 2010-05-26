using Ninject.Infrastructure.Disposal;
using Ninject.Parameters;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Integration
{
	public class WithPropertyValueTests : PropertyInjectionTests
	{
		[Fact]
		public void PropertyValueIsAssignedWhenNoInjectAttributeIsSuppliedUsingCallback()
		{
			_kernel.Bind<IWarrior>().To<FootSoldier>()
				.WithPropertyValue("Weapon", context => context.Kernel.Get<IWeapon>());
			var warrior = _kernel.Get<IWarrior>();
			ValidateWarrior(warrior);
		}

		[Fact]
		public void PropertyValueIsAssignedWhenNoInjectAttributeUsingSuppliedValue()
		{
			_kernel.Bind<IWarrior>().To<FootSoldier>()
				.WithPropertyValue("Weapon", _kernel.Get<IWeapon>());
			var warrior = _kernel.Get<IWarrior>();
			ValidateWarrior(warrior);
		}

#if !SILVERLIGHT
		[Fact]
		public void PropertyValuesOverrideDefaultBinding()
		{
			_kernel.Settings.InjectNonPublic = true;
			_kernel.Bind<IWarrior>().To<Ninja>()
				.WithPropertyValue("SecondaryWeapon", context => new Sword())
				.WithPropertyValue("SecretWeapon", context => new Sword());
			var warrior = _kernel.Get<IWarrior>();
			ValidateNinjaWarriorWithOverides(warrior);
		}
#endif //!SILVERLIGHT
	}

	public class WithParameterTests : PropertyInjectionTests
	{
		[Fact]
		public void PropertyValueIsAssignedWhenNoInjectAttributeIsSuppliedUsingCallback()
		{
			_kernel.Bind<IWarrior>().To<FootSoldier>()
				.WithParameter(new PropertyValue("Weapon", context => context.Kernel.Get<IWeapon>()));
			var warrior = _kernel.Get<IWarrior>();
			ValidateWarrior(warrior);
		}

		[Fact]
		public void PropertyValueIsAssignedWhenNoInjectAttributeUsingSuppliedValue()
		{
			_kernel.Bind<IWarrior>().To<FootSoldier>()
				.WithParameter(new PropertyValue("Weapon", _kernel.Get<IWeapon>()));
			var warrior = _kernel.Get<IWarrior>();
			ValidateWarrior(warrior);
		}

#if !SILVERLIGHT
		[Fact]
		public void PropertyValuesOverrideDefaultBinding()
		{
			_kernel.Settings.InjectNonPublic = true;
			_kernel.Bind<IWarrior>().To<Ninja>()
				.WithParameter(new PropertyValue("SecondaryWeapon", context => new Sword()))
				.WithParameter(new PropertyValue("SecretWeapon", context => new Sword()));
			var warrior = _kernel.Get<IWarrior>();
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
			_kernel.Settings.InjectNonPublic = true;
			_kernel.Bind<IWarrior>().To<Ninja>();
			var warrior = _kernel.Get<IWarrior>();
			Assert.IsType<Ninja>(warrior);
			Assert.IsType<Shuriken>(warrior.Weapon);
			Ninja ninja = warrior as Ninja;
			Assert.IsType<Shuriken>(ninja.SecondaryWeapon);
			Assert.IsType<Shuriken>(ninja.SecretWeaponAccessor);
        }
#endif //!SILVERLIGHT
    }

	public abstract class PropertyInjectionTests : DisposableObject
	{
		protected IKernel _kernel;

		public PropertyInjectionTests()
		{
			_kernel = new StandardKernel();
			_kernel.Bind<IWeapon>().To<Shuriken>();
		}

		protected void ValidateWarrior(IWarrior warrior)
		{
			Assert.IsType<FootSoldier>(warrior);
			Assert.NotNull(warrior.Weapon);
			Assert.IsType<Shuriken>(warrior.Weapon);
		}

		protected void ValidateNinjaWarriorWithOverides(IWarrior warrior)
		{
			Assert.IsType<Ninja>(warrior);
			Assert.IsType<Shuriken>(warrior.Weapon);
			Ninja ninja = warrior as Ninja;
			Assert.IsType<Sword>(ninja.SecondaryWeapon);
			Assert.IsType<Sword>(ninja.SecretWeaponAccessor);
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
}