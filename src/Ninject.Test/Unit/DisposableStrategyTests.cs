#if !NO_MOQ
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.DisposableStrategyTests
{
    using FluentAssertions;

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
            var reference = new InstanceReference { Instance = instance };

            strategy.Deactivate(contextMock.Object, reference);
            instance.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void StrategyDoesNotAttemptToDisposeInstanceIfItIsNotDisposable()
        {
            var instance = new object();
            var reference = new InstanceReference { Instance = instance };

            strategy.Deactivate(contextMock.Object, reference);
        }
    }
}
#endif