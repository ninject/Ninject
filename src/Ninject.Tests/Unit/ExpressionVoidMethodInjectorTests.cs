using System;
using System.Reflection;
using Ninject.Injection.Expressions;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.ExpressionVoidMethodInjectorTests
{
	public class ExpressionVoidMethodInjectorContext
	{
		protected MethodInfo method;
		protected ExpressionVoidMethodInjector injector;

		public ExpressionVoidMethodInjectorContext()
		{
			method = typeof(Samurai).GetMethod("SetName");
			injector = new ExpressionVoidMethodInjector(method);
		}
	}

	public class WhenVoidMethodInjectorIsCreated : ExpressionVoidMethodInjectorContext
	{
		[Fact]
		public void CanGetCallback()
		{
			injector.Callback.ShouldNotBeNull();
		}
	}

	public class WhenVoidMethodInjectorIsInvoked : ExpressionVoidMethodInjectorContext
	{
		[Fact]
		public void CallsMethod()
		{
			var samurai = new Samurai(new Sword());

			injector.Invoke(samurai, new[] { "Bob" });

			samurai.Name.ShouldBe("Bob");
		}

		[Fact]
		public void CallsMethodWithNullArgumentIfOneIsSpecified()
		{
			var samurai = new Samurai(new Sword());
			samurai.Name = "Bob";

			injector.Invoke(samurai, new[] { (string)null });

			samurai.Name.ShouldBeNull();
		}
	}
}
