#if !NO_MOQ
namespace Ninject.Tests.Unit
{
    using System;
    using FluentAssertions;
    using Moq;
    using Ninject.Activation.Caching;
    using Xunit;

    public class ActivationCacheTests
    {
        private ActivationCache testee;

        public ActivationCacheTests()
        {
            this.testee = new ActivationCache(new Mock<ICachePruner>().Object);
        }

        [Fact]
        public void IsActivatedReturnsFalseForObjectsNotInTheActivationCache()
        {
            var activated = this.testee.IsActivated(new object());

            activated.Should().BeFalse();
        }

        [Fact]
        public void IsActivatedReturnsTrueForObjectsInTheActivationCache()
        {
            var instance = new object();

            this.testee.AddActivatedInstance(instance);
            var activated = this.testee.IsActivated(instance);
            var activatedObjectCount = this.testee.ActivatedObjectCount;

            activated.Should().BeTrue();
            activatedObjectCount.Should().Be(1);
        }

        [Fact]
        public void IsDeactivatedReturnsFalseForObjectsNotInTheDeactivationCache()
        {
            var activated = this.testee.IsDeactivated(new object());

            activated.Should().BeFalse();
        }

        [Fact]
        public void IsDeactivatedReturnsTrueForObjectsInTheDeactivationCache()
        {
            var instance = new object();

            this.testee.AddDeactivatedInstance(instance);
            var deactivated = this.testee.IsDeactivated(instance);
            var deactivatedObjectCount = this.testee.DeactivatedObjectCount;

            deactivated.Should().BeTrue();
            deactivatedObjectCount.Should().Be(1);
        }

        [Fact]
        public void DeadObjectsAreRemoved()
        {
            this.testee.AddActivatedInstance(new object());
            this.testee.AddDeactivatedInstance(new object());
            GC.Collect();
            GC.Collect();
            this.testee.Prune();
            var activatedObjectCount = this.testee.ActivatedObjectCount;
            var deactivatedObjectCount = this.testee.DeactivatedObjectCount;

            activatedObjectCount.Should().Be(0);
            deactivatedObjectCount.Should().Be(0);
        }
    }
}
#endif
