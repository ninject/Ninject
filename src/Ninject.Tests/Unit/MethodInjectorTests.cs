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
			Assert.NotNull(injector.Callback);
		}
	}

	public class WhenMethodInjectorIsInvoked : MethodInjectorContext
	{
		[Fact]
		public void CallsMethod()
		{
			var samurai = new Samurai(new Sword());
			injector.Invoke(samurai, new[] { "evildoer" });
			Assert.True(samurai.IsBattleHardened);
		}

		[Fact]
		public void ReturnsValueFromMethod()
		{
			var samurai = new Samurai(new Sword());
			var result = injector.Invoke(samurai, new[] { "evildoer" }) as string;
			Assert.Equal("Attacked evildoer with a sword", result);
		}
	}
}
