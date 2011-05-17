using System;
using System.Reflection;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Ninject.Tests.Fakes;
using Xunit;
using Ninject.Injection;

namespace Ninject.Tests.Unit.MethodInjectionDirectiveBaseTests
{
    using FluentAssertions;

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

            targets.Length.Should().Be(3);
            targets[0].Name.Should().Be("foo");
            targets[0].Type.Should().Be(typeof(int));
            targets[1].Name.Should().Be("bar");
            targets[1].Type.Should().Be(typeof(string));
            targets[2].Name.Should().Be("baz");
            targets[2].Type.Should().Be(typeof(IWeapon));
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