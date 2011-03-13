namespace Ninject.Tests.Unit.KernelTargetResolutionStrategyTests
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

    public class KernelTargetResolutionStrategyContext
    {
        protected KernelTargetResolutionStrategy Strategy { get; private set; }
        protected Mock<IContext> ContextMock { get; private set; }
        protected Mock<ITarget> TargetMock { get; private set; }
        protected Mock<IKernel> KernelMock { get; private set; }
        protected IList<IWeapon> RegisteredTypes { get; private set; }
        protected Type RequestedType { get; set; }
        protected Maybe<object> Result { get; private set; }

        public KernelTargetResolutionStrategyContext()
        {
            Strategy = new KernelTargetResolutionStrategy();
            this.ContextMock = new Mock<IContext>();
            this.KernelMock = new Mock<IKernel>();
            this.TargetMock = new Mock<ITarget>();
            this.TargetMock.SetupGet(t => t.Type).Returns(() => this.RequestedType);
            this.TargetMock.SetupGet(t => t.Member).Returns(() => typeof(Ninja).GetConstructors().First());
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

    public class WhenNoMatchingTypesAreRegistered : KernelTargetResolutionStrategyContext
    {
        public WhenNoMatchingTypesAreRegistered()
        {
            this.RegisteredTypes.Clear();
            this.RequestedType = typeof(IWeapon);
            this.Resolve();
        }

        [Fact]
        public void TheResultShouldBeNone()
        {
            this.Result.ShouldBe(Maybe<object>.None);
        }
    }

    public class WhenOneMatchingTypeIsRegistered : KernelTargetResolutionStrategyContext
    {
        private readonly Shuriken expectedValue = new Shuriken();

        public WhenOneMatchingTypeIsRegistered()
        {
            this.RegisteredTypes.Add(this.expectedValue);
            this.RequestedType = typeof(IWeapon);
            this.Resolve();
        }

        [Fact]
        public void ASingleValueShouldBeReturned()
        {
            ((IWeapon)Result.Value).ShouldBe(this.expectedValue);
        }
    }

    public class WhenMultipleMatchingTypesAreRegistered : KernelTargetResolutionStrategyContext
    {
        public WhenMultipleMatchingTypesAreRegistered()
        {
            this.RegisteredTypes.Add(new Shuriken());
            this.RegisteredTypes.Add(new Sword());
            this.RequestedType = typeof(IWeapon);
        }

        [Fact]
        public void AnActivationExceptionShouldBeThrown()
        {
            Assert.Throws<ActivationException>(() => this.Resolve());
        }
    }
}