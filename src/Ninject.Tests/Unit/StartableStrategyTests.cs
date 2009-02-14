using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Xunit;

namespace Ninject.Tests.Unit.StartableStrategyTests
{
	public class StartableStrategyContext
	{
		protected readonly StartableStrategy strategy;
		protected readonly Mock<IContext> contextMock;

		public StartableStrategyContext()
		{
			contextMock = new Mock<IContext>();
			strategy = new StartableStrategy();
		}
	}

	public class WhenActivateIsCalled : StartableStrategyContext
	{
		[Fact]
		public void StrategyStartsInstanceIfItIsStartable()
		{
			var instance = new StartableObject();

			contextMock.SetupGet(x => x.Instance).Returns(instance);
			strategy.Activate(contextMock.Object);

			Assert.True(instance.WasStarted);
		}

		[Fact]
		public void StrategyDoesNotAttemptToStartInstanceIfItIsNotStartable()
		{
			var instance = new object();

			contextMock.SetupGet(x => x.Instance).Returns(instance);
			strategy.Activate(contextMock.Object);
		}
	}

	public class WhenDeactivateIsCalled : StartableStrategyContext
	{
		[Fact]
		public void StrategyStopsInstanceIfItIsStartable()
		{
			var instance = new StartableObject();

			contextMock.SetupGet(x => x.Instance).Returns(instance);
			strategy.Deactivate(contextMock.Object);

			Assert.True(instance.WasStopped);
		}

		[Fact]
		public void StrategyDoesNotAttemptToInitializeInstanceIfItIsNotInitializable()
		{
			var instance = new object();

			contextMock.SetupGet(x => x.Instance).Returns(instance);
			strategy.Deactivate(contextMock.Object);
		}
	}

	public class StartableObject : IStartable
	{
		public bool WasStarted { get; set; }
		public bool WasStopped { get; set; }

		public void Start()
		{
			WasStarted = true;
		}

		public void Stop()
		{
			WasStopped = true;
		}
	}
}