#if !NO_MOQ
using System.Linq;
using System.Reflection;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
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
            strategy = new MethodInjectionStrategy();
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
            reference = new InstanceReference { Instance = instance };

            contextMock = new Mock<IContext>();
            planMock = new Mock<IPlan>();
            injector1 = (x, args) => { injector1WasCalled = true; };
            injector2 = (x, args) => { injector2WasCalled = true; };

            directives = new[]
            {
                new FakeMethodInjectionDirective(method1, injector1),
                new FakeMethodInjectionDirective(method2, injector2)
            };

            contextMock.SetupGet(x => x.Plan).Returns(planMock.Object);

            planMock.Setup(x => x.GetAll<MethodInjectionDirective>()).Returns(directives);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            strategy.Activate(contextMock.Object, reference);

            planMock.Verify(x => x.GetAll<MethodInjectionDirective>());
        }

        [Fact]
        public void CreatesMethodInjectorsForEachDirective()
        {
            strategy.Activate(contextMock.Object, reference);
        }

        [Fact]
        public void ResolvesValuesForEachTargetOfEachDirective()
        {
            strategy.Activate(contextMock.Object, reference);

            directives.Map(d => d.TargetMocks.Map(m => m.Verify(x => x.ResolveWithin(contextMock.Object))));
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            strategy.Activate(contextMock.Object, reference);
            injector1WasCalled.Should().BeTrue();
            injector2WasCalled.Should().BeTrue();
        }
    }

    public class FakeMethodInjectionDirective : MethodInjectionDirective
    {
        public Mock<ITarget>[] TargetMocks { get; private set; }

        public FakeMethodInjectionDirective(MethodInfo method, MethodInjector injector)
            : base(method, injector) { }

        protected override ITarget[] CreateTargetsFromParameters(MethodInfo method)
        {
            TargetMocks = method.GetParameters().Select(p => new Mock<ITarget>()).ToArray();
            return TargetMocks.Select(m => m.Object).ToArray();
        }
    }

    public class Dummy
    {
        public void Foo(int a, string b) { }
        public void Bar(IWeapon weapon) { }
    }
}
#endif
