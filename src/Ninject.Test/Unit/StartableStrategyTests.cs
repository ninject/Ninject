#if !NO_MOQ
using Moq;
using Ninject.Activation;
using Ninject.Activation.Strategies;
using Xunit;

namespace Ninject.Tests.Unit.StartableStrategyTests
{
    using FluentAssertions;

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
            var reference = new InstanceReference { Instance = instance };

            strategy.Activate(contextMock.Object, reference);
            instance.WasStarted.Should().BeTrue();
        }

        [Fact]
        public void StrategyDoesNotAttemptToStartInstanceIfItIsNotStartable()
        {
            var instance = new object();
            var reference = new InstanceReference { Instance = instance };

            strategy.Activate(contextMock.Object, reference);
        }
    }

    public class WhenDeactivateIsCalled : StartableStrategyContext
    {
        [Fact]
        public void StrategyStopsInstanceIfItIsStartable()
        {
            var instance = new StartableObject();
            var reference = new InstanceReference { Instance = instance };

            strategy.Deactivate(contextMock.Object, reference);

            instance.WasStopped.Should().BeTrue();
        }

        [Fact]
        public void StrategyDoesNotAttemptToInitializeInstanceIfItIsNotInitializable()
        {
            var instance = new object();
            var reference = new InstanceReference { Instance = instance };

            strategy.Deactivate(contextMock.Object, reference);
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
#endif