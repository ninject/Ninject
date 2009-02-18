using System;
using System.Reflection;
using Ninject.Injection.Linq;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.MethodInjectorTests
{
	public class MethodInjectorContext
	{
		protected MethodInfo method;
		protected MethodInjector injector;

		public MethodInjectorContext()
		{
			method = typeof(Samurai).GetMethod("Attack");
			injector = new MethodInjector(method);
		}
	}

	public class WhenMethodInjectorIsCreated : MethodInjectorContext
	{
		[Fact]
		public void CanGetCallback()
		{
			injector.Callback.ShouldNotBeNull();
		}
	}

	public class WhenMethodInjectorIsInvoked : MethodInjectorContext
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
