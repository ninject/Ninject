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
		protected Mock<IPropertyInjector> propertyInjectorMock1;
		protected Mock<IPropertyInjector> propertyInjectorMock2;
		protected FakePropertyInjectionDirective[] directives;

		public WhenActivateIsCalled()
		{
			contextMock = new Mock<IContext>();
			planMock = new Mock<IPlan>();
			propertyInjectorMock1 = new Mock<IPropertyInjector>();
			propertyInjectorMock2 = new Mock<IPropertyInjector>();

			directives = new[] { new FakePropertyInjectionDirective(property1), new FakePropertyInjectionDirective(property2) };

			injectorFactoryMock.Setup(x => x.GetPropertyInjector(property1)).Returns(propertyInjectorMock1.Object).AtMostOnce();
			injectorFactoryMock.Setup(x => x.GetPropertyInjector(property2)).Returns(propertyInjectorMock2.Object).AtMostOnce();

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

			injectorFactoryMock.Verify(x => x.GetPropertyInjector(property1));
			injectorFactoryMock.Verify(x => x.GetPropertyInjector(property2));
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

			propertyInjectorMock1.Verify(x => x.Invoke(instance, null));
			propertyInjectorMock2.Verify(x => x.Invoke(instance, null));
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