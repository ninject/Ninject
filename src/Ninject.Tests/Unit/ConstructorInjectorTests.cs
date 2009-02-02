using System;
using System.Reflection;
using Ninject.Injection.Injectors.Linq;
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
			Assert.NotNull(injector.Callback);
		}
	}

	public class WhenConstructorInjectorIsInvoked : ConstructorInjectorContext
	{
		[Fact]
		public void CallsConstructor()
		{
			var sword = new Sword();
			var samurai = injector.Invoke(sword) as Samurai;
			Assert.Same(sword, samurai.Weapon);
		}

		[Fact]
		public void CallsConstructorWithNullArgumentIfOneIsSpecified()
		{
			var samurai = injector.Invoke((IWeapon)null) as Samurai;
			Assert.NotNull(samurai);
		}
	}
}
