using System;
using System.Reflection;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;
using Ninject.Injection;

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
			MethodInjector injector = delegate { };

			directive = new FakeMethodInjectionDirective(method, injector);
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

	public class FakeMethodInjectionDirective : MethodInjectionDirectiveBase<MethodInfo, MethodInjector>
	{
		public FakeMethodInjectionDirective(MethodInfo method, MethodInjector injector) : base(method, injector) { }
	}

	public class Dummy
	{
		public void MethodA(int foo, string bar, IWeapon baz) { }
	}
}