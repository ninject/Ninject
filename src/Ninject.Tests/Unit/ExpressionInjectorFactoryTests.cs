using System;
using System.Reflection;
using Ninject.Injection;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.ExpressionInjectorFactoryTests
{
	public class ExpressionInjectorFactoryContext
	{
		protected ExpressionInjectorFactory injectorFactory;

		public ExpressionInjectorFactoryContext()
		{
			injectorFactory = new ExpressionInjectorFactory();
		}
	}

	public class WhenConstructorInjectorIsInvoked : ExpressionInjectorFactoryContext
	{
		protected ConstructorInfo constructor;
		protected ConstructorInjector injector;

		public WhenConstructorInjectorIsInvoked()
		{
			constructor = typeof(Samurai).GetConstructor(new[] { typeof(IWeapon) });
			injector = injectorFactory.Create(constructor);
		}

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

	public class WhenPropertyInjectorIsInvoked : ExpressionInjectorFactoryContext
	{
		protected PropertyInfo property;
		protected PropertyInjector injector;

		public WhenPropertyInjectorIsInvoked()
		{
			property = typeof(Samurai).GetProperty("Weapon");
			injector = injectorFactory.Create(property);
		}

		[Fact]
		public void SetsPropertyValue()
		{
			var samurai = new Samurai(null);
			var sword = new Sword();

			injector.Invoke(samurai, sword);

			samurai.Weapon.ShouldBeSameAs(sword);
		}

		[Fact]
		public void SetsPropertyValueToNullIfInvokedWithNullArgument()
		{
			var samurai = new Samurai(new Sword());
			injector.Invoke(samurai, null);
			samurai.Weapon.ShouldBeNull();
		}
	}

	public class WhenMethodInjectorIsInvokedOnVoidMethod : ExpressionInjectorFactoryContext
	{
		protected MethodInfo method;
		protected MethodInjector injector;

		public WhenMethodInjectorIsInvokedOnVoidMethod()
		{
			method = typeof(Samurai).GetMethod("SetName");
			injector = injectorFactory.Create(method);
		}

		[Fact]
		public void CallsMethod()
		{
			var samurai = new Samurai(new Sword());
			injector.Invoke(samurai, new[] { "Bob" });
			samurai.Name.ShouldBe("Bob");
		}
	}

	public class WhenMethodInjectorIsInvokedOnNonVoidMethod : ExpressionInjectorFactoryContext
	{
		protected MethodInfo method;
		protected MethodInjector injector;

		public WhenMethodInjectorIsInvokedOnNonVoidMethod()
		{
			method = typeof(Samurai).GetMethod("Attack");
			injector = injectorFactory.Create(method);
		}

		[Fact]
		public void CallsMethod()
		{
			var samurai = new Samurai(new Sword());
			injector.Invoke(samurai, new[] { "evildoer" });
			samurai.IsBattleHardened.ShouldBeTrue();
		}
	}
}
