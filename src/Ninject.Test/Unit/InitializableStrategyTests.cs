using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Xunit;

namespace Ninject.Tests.Unit.InitializableStrategyTests
{
    using FluentAssertions;

    public class InitializableStrategyContext
    {
        protected readonly InitializableStrategy strategy;
        protected readonly Mock<IContext> contextMock;

        public InitializableStrategyContext()
        {
            this.contextMock = new Mock<IContext>();
            this.strategy = new InitializableStrategy();
        }
    }

    public class WhenActivateIsCalled : InitializableStrategyContext
    {
        [Fact]
        public void StrategyInitializesInstanceIfItIsInitializable()
        {
            var instance = new InitializableObject();
            var reference = new InstanceReference { Instance = instance };

            this.strategy.Activate(this.contextMock.Object, reference);
            instance.WasInitialized.Should().BeTrue();
        }

        [Fact]
        public void StrategyDoesNotAttemptToInitializeInstanceIfItIsNotInitializable()
        {
            var instance = new object();
            var reference = new InstanceReference { Instance = instance };

            this.strategy.Activate(this.contextMock.Object, reference);
        }
    }

    public class InitializableObject : IInitializable
    {
        public bool WasInitialized { get; set; }

        public void Initialize()
        {
            this.WasInitialized = true;
        }
    }
}