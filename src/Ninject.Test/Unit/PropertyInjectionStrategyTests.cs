using System.Reflection;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Infrastructure.Language;
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
            this.strategy = new PropertyInjectionStrategy(null) {Settings = new NinjectSettings()};
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
            this.contextMock = new Mock<IContext>();
            this.planMock = new Mock<IPlan>();
            this.injector1 = (x, y) => { this.injector1WasCalled = true; };
            this.injector2 = (x, y) => { this.injector2WasCalled = true; };

            this.directives = new[]
            {
                new FakePropertyInjectionDirective(this.property1, this.injector1),
                new FakePropertyInjectionDirective(this.property2, this.injector2)
            };

            this.contextMock.SetupGet(x => x.Plan).Returns(this.planMock.Object);
            this.contextMock.SetupGet(x => x.Parameters).Returns(new IParameter[0]);

            this.reference = new InstanceReference { Instance = this.instance };

            this.planMock.Setup(x => x.GetAll<PropertyInjectionDirective>()).Returns(this.directives);
        }

        [Fact]
        public void ReadsMethodInjectorsFromPlan()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.planMock.Verify(x => x.GetAll<PropertyInjectionDirective>());
        }

        [Fact]
        public void ResolvesValuesForEachTargetOfEachDirective()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);

            this.directives.Map(d => d.TargetMock.Verify(x => x.ResolveWithin(this.contextMock.Object)));
        }

        [Fact]
        public void InvokesInjectorsForEachDirective()
        {
            this.strategy.Activate(this.contextMock.Object, this.reference);
            this.injector1WasCalled.Should().BeTrue();
            this.injector2WasCalled.Should().BeTrue();
        }
    }

    public class FakePropertyInjectionDirective : PropertyInjectionDirective
    {
        public Mock<ITarget> TargetMock { get; private set; }

        public FakePropertyInjectionDirective(PropertyInfo property, PropertyInjector injector)
            : base(property, injector) { }

        protected override ITarget CreateTarget(PropertyInfo property)
        {
            this.TargetMock = new Mock<ITarget>();
            return this.TargetMock.Object;
        }
    }

    public class Dummy
    {
        public int Foo { get; set; }
        public string Bar { get; set; }
    }
}