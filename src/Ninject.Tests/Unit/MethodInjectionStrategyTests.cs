using System;
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
	public class MethodInjectionStrategyContext
	{
		protected Mock<IInjectorFactory> injectorFactoryMock;
		protected readonly MethodInjectionStrategy strategy;

		public MethodInjectionStrategyContext()
		{
			injectorFactoryMock = new Mock<IInjectorFactory>();
			strategy = new MethodInjectionStrategy(injectorFactoryMock.Object);
		}
	}

	public class WhenActivateIsCalled : MethodInjectionStrategyContext
	{
		protected Dummy instance = new Dummy();
		protected MethodInfo method1 = typeof(Dummy).GetMethod("Foo");
		protected MethodInfo method2 = typeof(Dummy).GetMethod("Bar");
		protected Mock<IContext> contextMock;
		protected Mock<IPlan> planMock;
		protected Mock<IMethodInjector> methodInjectorMock1;
		protected Mock<IMethodInjector> methodInjectorMock2;
		protected FakeMethodInjectionDirective[] directives;

		public WhenActivateIsCalled()
		{
			contextMock = new Mock<IContext>();
			planMock = new Mock<IPlan>();
			methodInjectorMock1 = new Mock<IMethodInjector>();
			methodInjectorMock2 = new Mock<IMethodInjector>();

			directives = new[] { new FakeMethodInjectionDirective(method1), new FakeMethodInjectionDirective(method2) };

			injectorFactoryMock.Setup(x => x.GetMethodInjector(method1)).Returns(methodInjectorMock1.Object).AtMostOnce();
			injectorFactoryMock.Setup(x => x.GetMethodInjector(method2)).Returns(methodInjectorMock2.Object).AtMostOnce();

			contextMock.SetupGet(x => x.Plan).Returns(planMock.Object);
			contextMock.SetupGet(x => x.Instance).Returns(instance);

			planMock.Setup(x => x.GetAll<MethodInjectionDirective>()).Returns(directives);
		}

		[Fact]
		public void ReadsMethodInjectorsFromPlan()
		{
			strategy.Activate(contextMock.Object);

			planMock.Verify(x => x.GetAll<MethodInjectionDirective>());
		}

		[Fact]
		public void CreatesMethodInjectorsForEachDirective()
		{
			strategy.Activate(contextMock.Object);

			injectorFactoryMock.Verify(x => x.GetMethodInjector(method1));
			injectorFactoryMock.Verify(x => x.GetMethodInjector(method2));
		}

		[Fact]
		public void ResolvesValuesForEachTargetOfEachDirective()
		{
			strategy.Activate(contextMock.Object);

			directives.Map(d => d.TargetMocks.Map(m => m.Verify(x => x.ResolveWithin(contextMock.Object))));
		}

		[Fact]
		public void InvokesInjectorsForEachDirective()
		{
			strategy.Activate(contextMock.Object);

			methodInjectorMock1.Verify(x => x.Invoke(instance, It.Is<object[]>(a => a.Length == 2)));
			methodInjectorMock2.Verify(x => x.Invoke(instance, It.Is<object[]>(a => a.Length == 1)));
		}
	}

	public class FakeMethodInjectionDirective : MethodInjectionDirective
	{
		public Mock<ITarget>[] TargetMocks { get; private set; }

		public FakeMethodInjectionDirective(MethodInfo method) : base(method) { }

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
