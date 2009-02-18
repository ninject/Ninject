using System;
using System.Reflection;
using Ninject.Injection.Linq;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.ConstructorInjectorTests
{
	public class ConstructorInjectorContext
	{
		protected ConstructorInfo constructor;
		protected ConstructorInjector injector;

		public ConstructorInjectorContext()
		{
			constructor = typeof(Samurai).GetConstructor(new[] { typeof(IWeapon) });
			injector = new ConstructorInjector(constructor);
		}
	}

	public class WhenConstructorInjectorIsCreated : ConstructorInjectorContext
	{
		[Fact]
		public void CanGetCallback()
		{
			injector.Callback.ShouldNotBeNull();
		}
	}

	public class WhenConstructorInjectorIsInvoked : ConstructorInjectorContext
	{
		[Fact]
		public void CallsConstructor()
		{
			var sword = new Sword();

			var samurai = injector.Invoke(new[] { sword }) as Samurai;

			samurai.ShouldNotBeNull();
			samurai.Weapon.ShouldBeSameAs(sword);
		}

		[Fact]
		public void CallsConstructorWithNullArgumentIfOneIsSpecified()
		{
			var samurai = injector.Invoke(new[] { (IWeapon)null }) as Samurai;

			samurai.ShouldNotBeNull();
			samurai.Weapon.ShouldBeNull();
		}
	}
}
