namespace Ninject.Tests.Unit.DefaultValueTargetResolutionStrategyTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Parameters;
    using Ninject.Planning.Targets;
    using Ninject.Planning.Targets.Strategies;
    using Ninject.Tests.Fakes;
    using Xunit;
    using Xunit.Should;

    public class DefaultValueTargetResolutionStrategyContext
    {
        protected DefaultValueTargetResolutionStrategy Strategy { get; private set; }
        protected Mock<IContext> ContextMock { get; private set; }
        protected Mock<ITarget> TargetMock { get; private set; }
        protected Mock<IKernel> KernelMock { get; private set; }
        protected IList<IWeapon> RegisteredTypes { get; private set; }
        protected Type RequestedType { get; set; }
        protected Maybe<object> Result { get; private set; }

        public DefaultValueTargetResolutionStrategyContext()
        {
            Strategy = new DefaultValueTargetResolutionStrategy();
            this.ContextMock = new Mock<IContext>();
            this.KernelMock = new Mock<IKernel>();
            this.TargetMock = new Mock<ITarget>();
            this.TargetMock.SetupGet(t => t.Type).Returns(() => this.RequestedType);
            this.RegisteredTypes = new List<IWeapon>();

            this.ContextMock.SetupGet(c => c.Request).Returns(new Request(typeof(Ninja), null, new IParameter[0], null, false, false));
            this.ContextMock.SetupGet(c => c.Kernel).Returns(KernelMock.Object);
            this.ContextMock.SetupGet(c => c.Parameters).Returns(new List<IParameter>());
            this.KernelMock.Setup(k => k.Resolve(It.IsAny<IRequest>())).Returns(this.RegisteredTypes.Cast<object>());
        }

        protected void Resolve()
        {
            this.Result = this.Strategy.Resolve(this.TargetMock.Object, this.ContextMock.Object);
        }
    }

    public class WhenTargetDoesNotADefaultValue : DefaultValueTargetResolutionStrategyContext
    {
        public WhenTargetDoesNotADefaultValue()
        {
            this.Resolve();
        }

        [Fact]
        public void TheResultShoudBeNone()
        {
            this.Result.ShouldBe(Maybe<object>.None);
        }
    }

    public class WhenTargetHasADefaultValue : DefaultValueTargetResolutionStrategyContext
    {
        private const ShieldColor DefaultValue = ShieldColor.Orange;
        public WhenTargetHasADefaultValue()
        {
            this.TargetMock.SetupGet(t => t.HasDefaultValue).Returns(true);
            this.TargetMock.SetupGet(t => t.DefaultValue).Returns(DefaultValue);
            this.Resolve();
        }

        [Fact]
        public void DefaultValueShouldBeReturned()
        {
            this.Result.Value.ShouldBe(DefaultValue);
        }
    }
}