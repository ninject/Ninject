using System;
using System.Reflection;
using Ninject.Injection.Expressions;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.ExpressionConstructorInjectorTests
{
	public class ExpressionConstructorInjectorContext
	{
		protected ConstructorInfo constructor;
		protected ExpressionConstructorInjector injector;

		public ExpressionConstructorInjectorContext()
		{
			constructor = typeof(Samurai).GetConstructor(new[] { typeof(IWeapon) });
			injector = new ExpressionConstructorInjector(constructor);
		}
	}

	public class WhenConstructorInjectorIsCreated : ExpressionConstructorInjectorContext
	{
		[Fact]
		public void CanGetCallback()
		{
			injector.Callback.ShouldNotBeNull();
		}
	}

	public class WhenConstructorInjectorIsInvoked : ExpressionConstructorInjectorContext
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
