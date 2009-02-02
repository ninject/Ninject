using System;
using System.Reflection;
using Ninject.Injection.Injectors.Linq;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.PropertyInjectorTests
{
	public class PropertyInjectorContext
	{
		protected PropertyInfo property;
		protected PropertyInjector injector;

		public PropertyInjectorContext()
		{
			property = typeof(Samurai).GetProperty("Weapon");
			injector = new PropertyInjector(property);
		}
	}

	public class WhenPropertyInjectorIsCreated : PropertyInjectorContext
	{
		[Fact]
		public void CanGetCallback()
		{
			Assert.NotNull(injector.Callback);
		}
	}

	public class WhenPropertyInjectorIsInvoked : PropertyInjectorContext
	{
		[Fact]
		public void SetsPropertyValue()
		{
			var samurai = new Samurai(null);
			var sword = new Sword();
			injector.Invoke(samurai, sword);
			Assert.Same(sword, samurai.Weapon);
		}

		[Fact]
		public void SetsPropertyValueToNullIfInvokedWithNullArgument()
		{
			var samurai = new Samurai(new Sword());
			injector.Invoke(samurai, null);
			Assert.Null(samurai.Weapon);
		}
	}
}
