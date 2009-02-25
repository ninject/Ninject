using System;
using System.Reflection;
using Ninject.Injection.Expressions;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.ExpressionPropertyInjectorTests
{
	public class ExpressionPropertyInjectorContext
	{
		protected PropertyInfo property;
		protected ExpressionPropertyInjector injector;

		public ExpressionPropertyInjectorContext()
		{
			property = typeof(Samurai).GetProperty("Weapon");
			injector = new ExpressionPropertyInjector(property);
		}
	}

	public class WhenPropertyInjectorIsCreated : ExpressionPropertyInjectorContext
	{
		[Fact]
		public void CanGetCallback()
		{
			injector.Callback.ShouldNotBeNull();
		}
	}

	public class WhenPropertyInjectorIsInvoked : ExpressionPropertyInjectorContext
	{
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
}
