using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Infrastructure.Disposal;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.DisposableStrategyTests
{
	public class DisposableStrategyContext
	{
		protected readonly DisposableStrategy strategy;
		protected readonly Mock<IContext> contextMock;

		public DisposableStrategyContext()
		{
			contextMock = new Mock<IContext>();
			strategy = new DisposableStrategy();
		}
	}

	public class WhenDeactivateIsCalled : DisposableStrategyContext
	{
		[Fact]
		public void StrategyDisposesInstanceIfItIsDisposable()
		{
			var instance = new NotifiesWhenDisposed();

			contextMock.SetupGet(x => x.Instance).Returns(instance);
			strategy.Deactivate(contextMock.Object);

			instance.IsDisposed.ShouldBeTrue();
		}

		[Fact]
		public void StrategyDoesNotAttemptToDisposeInstanceIfItIsNotDisposable()
		{
			var instance = new object();

			contextMock.SetupGet(x => x.Instance).Returns(instance);
			strategy.Deactivate(contextMock.Object);
		}
	}
}