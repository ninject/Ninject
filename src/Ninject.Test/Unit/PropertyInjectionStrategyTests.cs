#if !NO_MOQ
using System.Reflection;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Xunit;

namespace Ninject.Tests.Unit.PropertyInjectionStrategyTests
{
    using FluentAssertions;

    public class PropertyInjectionDirectiveContext
    {
        protected readonly PropertyInjectionStrategy strategy;

        public PropertyInjectionDirectiveContext()
        {
            strategy = new PropertyInjectionStrategy(null) {Settings = new NinjectSettings()};
        }
    }

    public class WhenActivateIsCalled : PropertyInjectionDirectiveContext
    {
        protected Dummy instance = new Dummy();
        protected PropertyInfo property1 = typeof(Dummy).GetProperty("Foo");
        protected PropertyInfo property2 = typeof(Dummy).GetProperty("Bar");
        protected InstanceReference reference;
        protected Mock<IContext> contextMock;
        protected Mock<IPlan> planMock;
        protected FakePropertyInjectionDirective[] directives;
        protected PropertyInjector injector1;
        protected PropertyInjector injector2;
        protected bool injector1WasCalled;
        protected bool injector2WasCalled;

        public WhenActivateIsCalled()
        {
            contextMock = new Mock<IContext>();
            planMock = new Mock<IPlan>();
            injector1 = (x, y) => { injector1WasCalled = true; };
            injector2 = (x, y) => { injector2WasCalled = true; };

            directives = new[]
            {
                new FakePropertyInjectionDirective(property1, injector1),
                new FakePropertyInjectionDirective(property2, injector2)
            };

            contextMock.SetupGet(x => x.Plan).Returns(planMock.Object);
            contextMock.SetupGet(x => x.Parameters).Returns(new IParameter[0]);

            reference = new InstanceReference { Instance = instance };

            planMock.Setup(x => x.GetAll<PropertyInjectionDirective>()).Returns(directives);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            strategy.Activate(contextMock.Object, reference);

            planMock.Verify(x => x.GetAll<PropertyInjectionDirective>());
        }

        [Fact]
        public void ResolvesValuesForEachTargetOfEachDirective()
        {
            strategy.Activate(contextMock.Object, reference);

            directives.Map(d => d.TargetMock.Verify(x => x.ResolveWithin(contextMock.Object)));
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            strategy.Activate(contextMock.Object, reference);
            injector1WasCalled.Should().BeTrue();
            injector2WasCalled.Should().BeTrue();
        }
    }

    public class FakePropertyInjectionDirective : PropertyInjectionDirective
    {
        public Mock<ITarget> TargetMock { get; private set; }

        public FakePropertyInjectionDirective(PropertyInfo property, PropertyInjector injector)
            : base(property, injector) { }

        protected override ITarget CreateTarget(PropertyInfo property)
        {
            TargetMock = new Mock<ITarget>();
            return TargetMock.Object;
        }
    }

    public class Dummy
    {
        public int Foo { get; set; }
        public string Bar { get; set; }
    }
}
#endif