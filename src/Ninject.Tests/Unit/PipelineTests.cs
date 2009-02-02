using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Syntax;
using Xunit;

namespace Ninject.Tests.Unit.PipelineTests
{
	public class PipelineContext
	{
		protected readonly List<Mock<IActivationStrategy>> strategyMocks = new List<Mock<IActivationStrategy>>();
		protected readonly Pipeline pipeline;

		public PipelineContext()
		{
			strategyMocks.Add(new Mock<IActivationStrategy>());
			strategyMocks.Add(new Mock<IActivationStrategy>());
			strategyMocks.Add(new Mock<IActivationStrategy>());
			pipeline = new Pipeline(strategyMocks.Select(mock => mock.Object));
		}
	}

	public class WhenPipelineIsCreated : PipelineContext
	{
		[Fact]
		public void HasListOfStrategies()
		{
			Assert.NotNull(pipeline.Strategies);

			for (int idx = 0; idx < strategyMocks.Count; idx++)
				Assert.Same(strategyMocks[idx].Object, pipeline.Strategies[idx]);
		}
	}

	public class WhenActivateIsCalled : PipelineContext
	{
		[Fact]
		public void CallsActivateOnStrategies()
		{
			var contextMock = new Mock<IContext>();

			strategyMocks.Map(mock => mock.Setup(x => x.Activate(contextMock.Object)).AtMostOnce());

			pipeline.Activate(contextMock.Object);

			strategyMocks.Map(mock => mock.Verify(x => x.Activate(contextMock.Object)));
		}
	}

	public class WhenDeactivateIsCalled : PipelineContext
	{
		[Fact]
		public void CallsDeactivateOnStrategies()
		{
			var contextMock = new Mock<IContext>();

			strategyMocks.Map(mock => mock.Setup(x => x.Deactivate(contextMock.Object)).AtMostOnce());

			pipeline.Deactivate(contextMock.Object);

			strategyMocks.Map(mock => mock.Verify(x => x.Deactivate(contextMock.Object)));
		}
	}
}