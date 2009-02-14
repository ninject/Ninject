using System;
using System.Reflection;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.MethodInjectionDirectiveBaseTests
{
	public class MethodInjectionDirectiveBaseContext
	{
		protected FakeMethodInjectionDirective directive;
	}

	public class WhenDirectiveIsCreated : MethodInjectionDirectiveBaseContext
	{
		[Fact]
		public void CreatesTargetsForMethodParameters()
		{
			var method = typeof(Dummy).GetMethod("MethodA");
			directive = new FakeMethodInjectionDirective(method);
			ITarget[] targets = directive.Targets;

			Assert.Equal(3, targets.Length);
			Assert.Equal(targets[0].Name, "foo");
			Assert.Equal(targets[0].Type, typeof(int));
			Assert.Equal(targets[1].Name, "bar");
			Assert.Equal(targets[1].Type, typeof(string));
			Assert.Equal(targets[2].Name, "baz");
			Assert.Equal(targets[2].Type, typeof(IWeapon));
		}
	}

	public class FakeMethodInjectionDirective : MethodInjectionDirectiveBase<MethodInfo>
	{
		public FakeMethodInjectionDirective(MethodInfo method) : base(method) { }
	}

	public class Dummy
	{
		public void MethodA(int foo, string bar, IWeapon baz) { }
	}
}