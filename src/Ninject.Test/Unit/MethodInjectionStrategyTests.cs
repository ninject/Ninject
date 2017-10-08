using System.Linq;
using System.Reflection;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Infrastructure.Language;
using Ninject.Injection;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.MethodInjectionStrategyTests
{
    using FluentAssertions;

    public class MethodInjectionStrategyContext
    {
        protected readonly MethodInjectionStrategy strategy;

        public MethodInjectionStrategyContext()
        {
            this.strategy = new MethodInjectionStrategy();
        }
    }

    public class WhenActivateIsCalled : MethodInjectionStrategyContext
    {
        protected Dummy instance = new Dummy();
        protected InstanceReference reference;
        protected MethodInfo method1 = typeof(Dummy).GetMethod("Foo");
        protected MethodInfo method2 = typeof(Dummy).GetMethod("Bar");
        protected Mock<IContext> contextMock;
        protected Mock<IPlan> planMock;
        protected FakeMethodInjectionDirective[] directives;
        protected MethodInjector injector1;
        protected MethodInjector injector2;
        protected bool injector1WasCalled;
        protected bool injector2WasCalled;

        public WhenActivateIsCalled()
        {
            this.reference = new InstanceReference { Instance = this.instance };

            this.contextMock = new Mock<IContext>();
            this.planMock = new Mock<IPlan>();
            this.injector1 = (x, args) => { this.injector1WasCalled = true; };
            this.injector2 = (x, args) => { this.injector2WasCalled = true; };

            this.directives = new[]
            {
                new FakeMethodInjectionDirective(this.method1, this.injector1),
                new FakeMethodInjectionDirective(this.method2, this.injector2)
            };

            this.contextMock.SetupGet(x => x.Plan).Returns(this.planMock.Object);

            this.planMock.Setup(x => x.GetAll<MethodInjectionDirective>()).Returns(this.directives);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.planMock.Verify(x => x.GetAll<MethodInjectionDirective>());
        }

        [Fact]
        public void CreatesMethodInjectorsForEachDirective()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);
        }

        [Fact]
        public void ResolvesValuesForEachTargetOfEachDirective()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.directives.Map(d => d.TargetMocks.Map(m => m.Verify(x => x.ResolveWithin(this.contextMock.Object))));
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);
            this.injector1WasCalled.Should().BeTrue();
            this.injector2WasCalled.Should().BeTrue();
        }
    }

    public class FakeMethodInjectionDirective : MethodInjectionDirective
    {
        public Mock<ITarget>[] TargetMocks { get; private set; }

        public FakeMethodInjectionDirective(MethodInfo method, MethodInjector injector)
            : base(method, injector) { }

        protected override ITarget[] CreateTargetsFromParameters(MethodInfo method)
        {
            this.TargetMocks = method.GetParameters().Select(p => new Mock<ITarget>()).ToArray();
            return this.TargetMocks.Select(m => m.Object).ToArray();
        }
    }

    public class Dummy
    {
        public void Foo(int a, string b) { }
        public void Bar(IWeapon weapon) { }
    }
}