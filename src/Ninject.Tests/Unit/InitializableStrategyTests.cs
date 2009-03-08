using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.InitializableStrategyTests
{
	public class InitializableStrategyContext
	{
		protected readonly InitializableStrategy strategy;
		protected readonly Mock<IContext> contextMock;

		public InitializableStrategyContext()
		{
			contextMock = new Mock<IContext>();
			strategy = new InitializableStrategy();
		}
	}

	public class WhenActivateIsCalled : InitializableStrategyContext
	{
		[Fact]
		public void StrategyInitializesInstanceIfItIsInitializable()
		{
			var instance = new InitializableObject();

			contextMock.SetupGet(x => x.Instance).Returns(instance);
			strategy.Activate(contextMock.Object);

			instance.WasInitialized.ShouldBeTrue();
		}

		[Fact]
		public void StrategyDoesNotAttemptToInitializeInstanceIfItIsNotInitializable()
		{
			var instance = new object();

			contextMock.SetupGet(x => x.Instance).Returns(instance);
			strategy.Activate(contextMock.Object);
		}
	}

	public class InitializableObject : IInitializable
	{
		public bool WasInitialized { get; set; }

		public void Initialize()
		{
			WasInitialized = true;
		}
	}
}