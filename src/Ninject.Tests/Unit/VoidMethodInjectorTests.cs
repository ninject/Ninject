using System;
using System.Reflection;
using Ninject.Injection.Linq;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.VoidMethodInjectorTests
{
	public class VoidMethodInjectorContext
	{
		protected MethodInfo method;
		protected VoidMethodInjector injector;

		public VoidMethodInjectorContext()
		{
			method = typeof(Samurai).GetMethod("SetName");
			injector = new VoidMethodInjector(method);
		}
	}

	public class WhenVoidMethodInjectorIsCreated : VoidMethodInjectorContext
	{
		[Fact]
		public void CanGetCallback()
		{
			injector.Callback.ShouldNotBeNull();
		}
	}

	public class WhenVoidMethodInjectorIsInvoked : VoidMethodInjectorContext
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
