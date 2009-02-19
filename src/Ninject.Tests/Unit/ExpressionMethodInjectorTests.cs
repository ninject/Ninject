using System;
using System.Reflection;
using Ninject.Injection.Expressions;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.ExpressionMethodInjectorTests
{
	public class ExpressionMethodInjectorContext
	{
		protected MethodInfo method;
		protected ExpressionMethodInjector injector;

		public ExpressionMethodInjectorContext()
		{
			method = typeof(Samurai).GetMethod("Attack");
			injector = new ExpressionMethodInjector(method);
		}
	}

	public class WhenMethodInjectorIsCreated : ExpressionMethodInjectorContext
	{
		[Fact]
		public void CanGetCallback()
		{
			injector.Callback.ShouldNotBeNull();
		}
	}

	public class WhenMethodInjectorIsInvoked : ExpressionMethodInjectorContext
	{
		[Fact]
		public void CallsMethod()
		{
			var samurai = new Samurai(new Sword());

			injector.Invoke(samurai, new[] { "evildoer" });

			samurai.IsBattleHardened.ShouldBeTrue();
		}

		[Fact]
		public void ReturnsValueFromMethod()
		{
			var samurai = new Samurai(new Sword());

			var result = injector.Invoke(samurai, new[] { "evildoer" }) as string;

			result.ShouldBe("Attacked evildoer with a sword");
		}
	}
}
