namespace Ninject.Tests.Unit
{
    using System;

    using Moq;

    using Ninject.Activation.Caching;
    using Xunit;
    using Xunit.Should;

    public class ActivationCacheTests
    {
        private readonly ActivationCache testee;

        public ActivationCacheTests()
        {
            this.testee = new ActivationCache(new Mock<ICachePruner>().Object);
        }

        [Fact]
        public void IsActivatedReturnsFalseForObjectsNotInTheActivationCache()
        {
            var activated = this.testee.IsActivated(new object());

            activated.ShouldBeFalse();
        }

        [Fact]
        public void IsActivatedReturnsTrueForObjectsInTheActivationCache()
        {
            var instance = new object();

            this.testee.AddActivatedInstance(instance);
            var activated = this.testee.IsActivated(instance);
            var activatedObjectCount = this.testee.ActivatedObjectCount;

            activated.ShouldBeTrue();
            activatedObjectCount.ShouldBe(1);
        }

        [Fact]
        public void IsDeactivatedReturnsFalseForObjectsNotInTheDeactivationCache()
        {
            var activated = this.testee.IsDeactivated(new object());

            activated.ShouldBeFalse();
        }

        [Fact]
        public void IsDeactivatedReturnsTrueForObjectsInTheDeactivationCache()
        {
            var instance = new object();

            this.testee.AddDeactivatedInstance(instance);
            var deactivated = this.testee.IsDeactivated(instance);
            var deactivatedObjectCount = this.testee.DeactivatedObjectCount;

            deactivated.ShouldBeTrue();
            deactivatedObjectCount.ShouldBe(1);
        }
        
        [Fact]
        public void DeadObjectsAreRemoved()
        {
            this.testee.AddActivatedInstance(new object());
            this.testee.AddDeactivatedInstance(new object());
            GC.Collect();
            this.testee.Prune();
            var activatedObjectCount = this.testee.ActivatedObjectCount;
            var deactivatedObjectCount = this.testee.DeactivatedObjectCount;

            activatedObjectCount.ShouldBe(0);
            deactivatedObjectCount.ShouldBe(0);
        }
    }
}