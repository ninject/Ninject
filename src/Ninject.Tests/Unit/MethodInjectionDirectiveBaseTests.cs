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

			targets.Length.ShouldBe(3);
			targets[0].Name.ShouldBe("foo");
			targets[0].Type.ShouldBe(typeof(int));
			targets[1].Name.ShouldBe("bar");
			targets[1].Type.ShouldBe(typeof(string));
			targets[2].Name.ShouldBe("baz");
			targets[2].Type.ShouldBe(typeof(IWeapon));
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