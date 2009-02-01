using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Injection;
using Ninject.Injection.Injectors;
using Ninject.Injection.Injectors.Linq;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Syntax;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.MethodInjectionStrategyTests
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