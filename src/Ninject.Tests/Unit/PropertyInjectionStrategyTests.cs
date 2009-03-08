using System;
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
using Xunit.Should;

namespace Ninject.Tests.Unit.PropertyInjectionStrategyTests
{
	public class PropertyInjectionDirectiveContext
	{
		protected Mock<IInjectorFactory> injectorFactoryMock;
		protected readonly PropertyInjectionStrategy strategy;

		public PropertyInjectionDirectiveContext()
		{
			injectorFactoryMock = new Mock<IInjectorFactory>();
			strategy = new PropertyInjectionStrategy(injectorFactoryMock.Object);
		}
	}

	public class WhenActivateIsCalled : PropertyInjectionDirectiveContext
	{
		protected Dummy instance = new Dummy();
		protected PropertyInfo property1 = typeof(Dummy).GetProperty("Foo");
		protected PropertyInfo property2 = typeof(Dummy).GetProperty("Bar");
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

			directives = new[] { new FakePropertyInjectionDirective(property1), new FakePropertyInjectionDirective(property2) };

			injectorFactoryMock.Setup(x => x.GetInjector(property1)).Returns(injector1).AtMostOnce();
			injectorFactoryMock.Setup(x => x.GetInjector(property2)).Returns(injector2).AtMostOnce();

			contextMock.SetupGet(x => x.Plan).Returns(planMock.Object);
			contextMock.SetupGet(x => x.Instance).Returns(instance);
			contextMock.SetupGet(x => x.Parameters).Returns(new IParameter[0]);

			planMock.Setup(x => x.GetAll<PropertyInjectionDirective>()).Returns(directives);
		}

		[Fact]
		public void ReadsMethodInjectorsFromPlan()
		{
			strategy.Activate(contextMock.Object);

			planMock.Verify(x => x.GetAll<PropertyInjectionDirective>());
		}

		[Fact]
		public void CreatesMethodInjectorsForEachDirective()
		{
			strategy.Activate(contextMock.Object);

			injectorFactoryMock.Verify(x => x.GetInjector(property1));
			injectorFactoryMock.Verify(x => x.GetInjector(property2));
		}

		[Fact]
		public void ResolvesValuesForEachTargetOfEachDirective()
		{
			strategy.Activate(contextMock.Object);

			directives.Map(d => d.TargetMock.Verify(x => x.ResolveWithin(contextMock.Object)));
		}

		[Fact]
		public void InvokesInjectorsForEachDirective()
		{
			strategy.Activate(contextMock.Object);
			injector1WasCalled.ShouldBeTrue();
			injector2WasCalled.ShouldBeTrue();
		}
	}

	public class FakePropertyInjectionDirective : PropertyInjectionDirective
	{
		public Mock<ITarget> TargetMock { get; private set; }

		public FakePropertyInjectionDirective(PropertyInfo property) : base(property) { }

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