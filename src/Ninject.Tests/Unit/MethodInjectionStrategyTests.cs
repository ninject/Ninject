using System;
using System.Collections.Generic;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Injection;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Xunit;

namespace Ninject.Tests.Unit.MethodInjectionStrategyTests
{
	public class MethodInjectionStrategyContext
	{
		protected Mock<IKernel> kernelMock;
		protected Mock<IInjectorFactory> injectorFactoryMock;
		protected readonly MethodInjectionStrategy strategy;

		public MethodInjectionStrategyContext()
		{
			kernelMock = new Mock<IKernel>();
			injectorFactoryMock = new Mock<IInjectorFactory>();
			strategy = new MethodInjectionStrategy(kernelMock.Object, injectorFactoryMock.Object);
		}
	}

	public class WhenActivateIsCalled : MethodInjectionStrategyContext
	{
		[Fact]
		public void GetsAllMethodInjectionDirectivesFromPlan()
		{
			var contextMock = new Mock<IContext>();
			var planMock = new Mock<IPlan>();

			var directives = new List<MethodInjectionDirective>();

			contextMock.SetupGet(x => x.Plan).Returns(planMock.Object).AtMostOnce();
			planMock.Setup(x => x.GetAll<MethodInjectionDirective>()).Returns(directives).AtMostOnce();

			strategy.Activate(contextMock.Object);

			contextMock.VerifyGet(x => x.Plan);
			planMock.Verify(x => x.GetAll<MethodInjectionDirective>());
		}
	}
}