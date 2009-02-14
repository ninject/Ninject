using System;
using Ninject.Planning.Directives;
using Xunit;

namespace Ninject.Tests.Unit.PropertyInjectionDirectiveTests
{
	public class PropertyInjectionDirectiveContext
	{
		protected PropertyInjectionDirective directive;
	}

	public class WhenDirectiveIsCreated : PropertyInjectionDirectiveContext
	{
		[Fact]
		public void CreatesTargetForProperty()
		{
			var method = typeof(Dummy).GetProperty("Foo");
			directive = new PropertyInjectionDirective(method);

			Assert.Equal(directive.Target.Name, "Foo");
			Assert.Equal(directive.Target.Type, typeof(int));
		}
	}

	public class Dummy
	{
		public int Foo { get; set; }
	}
}