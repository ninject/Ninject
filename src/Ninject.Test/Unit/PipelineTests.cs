#if !NO_MOQ
namespace Ninject.Tests.Unit.PipelineTests
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Caching;    
    using Ninject.Activation.Strategies;
    using Xunit;

    public class PipelineContext
    {
        protected readonly List<Mock<IActivationStrategy>> StrategyMocks = new List<Mock<IActivationStrategy>>();
        protected readonly Mock<IActivationCache> ActivationCacheMock;
        protected readonly Pipeline Pipeline;

        public PipelineContext()
        {
            this.StrategyMocks.Add(new Mock<IActivationStrategy>());
            this.StrategyMocks.Add(new Mock<IActivationStrategy>());
            this.StrategyMocks.Add(new Mock<IActivationStrategy>());
            this.ActivationCacheMock = new Mock<IActivationCache>();
            this.Pipeline = new Pipeline(this.StrategyMocks.Select(mock => mock.Object), this.ActivationCacheMock.Object);
        }
    }

    public class WhenPipelineIsCreated : PipelineContext
    {
        [Fact]
        public void HasListOfStrategies()
        {
            this.Pipeline.Strategies.Should().NotBeNull();

            for (int idx = 0; idx < this.StrategyMocks.Count; idx++)
            {
                this.Pipeline.Strategies[idx].Should().BeSameAs(this.StrategyMocks[idx].Object);
            }
        }
    }

    public class WhenActivateIsCalled : PipelineContext
    {
        [Fact]
        public void CallsActivateOnStrategies()
        {
            var contextMock = new Mock<IContext>();
            var reference = new InstanceReference();

            this.Pipeline.Activate(contextMock.Object, reference);

            this.StrategyMocks.Map(mock => mock.Verify(x => x.Activate(contextMock.Object, reference)));
        }

        [Fact]
        public void WhenAlreadyActiavatedNothingHappens()
        {
            var contextMock = new Mock<IContext>();
            var reference = new InstanceReference();
            this.ActivationCacheMock.Setup(activationCache => activationCache.IsActivated(It.IsAny<object>())).Returns(true);

            this.Pipeline.Activate(contextMock.Object, reference);

            this.StrategyMocks.Map(mock => mock.Verify(x => x.Activate(contextMock.Object, reference), Times.Never()));
        }
    }

    public class WhenDeactivateIsCalled : PipelineContext
    {
        [Fact]
        public void CallsDeactivateOnStrategies()
        {
            var contextMock = new Mock<IContext>();
            var reference = new InstanceReference();

            this.Pipeline.Deactivate(contextMock.Object, reference);

            this.StrategyMocks.Map(mock => mock.Verify(x => x.Deactivate(contextMock.Object, reference)));
        }

        [Fact]
        public void WhenAlreadyDeactiavatedNothingHappens()
        {
            var contextMock = new Mock<IContext>();
            var reference = new InstanceReference();
            this.ActivationCacheMock.Setup(activationCache => activationCache.IsDeactivated(It.IsAny<object>())).Returns(true);

            this.Pipeline.Deactivate(contextMock.Object, reference);

            this.StrategyMocks.Map(mock => mock.Verify(x => x.Deactivate(contextMock.Object, reference), Times.Never()));
        }
    }
}
#endif