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
			Assert.NotNull(injector.Callback);
		}
	}

	public class WhenVoidMethodInjectorIsInvoked : VoidMethodInjectorContext
	{
		[Fact]
		public void CallsMethod()
		{
			var samurai = new Samurai(new Sword());
			injector.Invoke(samurai, "Bob");
			Assert.Equal("Bob", samurai.Name);
		}

		[Fact]
		public void CallsMethodWithNullArgumentIfOneIsSpecified()
		{
			var samurai = new Samurai(new Sword());
			samurai.Name = "Bob";
			injector.Invoke(samurai, (string)null);
			Assert.Null(samurai.Name);
		}
	}
}
