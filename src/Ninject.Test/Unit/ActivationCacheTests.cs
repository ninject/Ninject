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
        private readonly ActivationCache testee;

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
            var instance = new TestObject(42);

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
            var instance = new TestObject(42);

            this.testee.AddDeactivatedInstance(instance);
            var deactivated = this.testee.IsDeactivated(instance);
            var deactivatedObjectCount = this.testee.DeactivatedObjectCount;

            deactivated.Should().BeTrue();
            deactivatedObjectCount.Should().Be(1);
        }

        [Fact]
        public void DeadObjectsAreRemoved()
        {
            this.testee.AddActivatedInstance(new TestObject(42));
            this.testee.AddDeactivatedInstance(new TestObject(42));
            GC.Collect();
            GC.Collect();
            this.testee.Prune();
            var activatedObjectCount = this.testee.ActivatedObjectCount;
            var deactivatedObjectCount = this.testee.DeactivatedObjectCount;

            activatedObjectCount.Should().Be(0);
            deactivatedObjectCount.Should().Be(0);
        }

        [Fact]
        public void ImplementationDoesNotRelyOnObjectHashCode()
        {
            var instance = new TestObject(42);

            this.testee.AddActivatedInstance(instance);
            instance.ChangeHashCode(43);
            var isActivated = this.testee.IsActivated(instance);

            isActivated.Should().BeTrue();
        }
    }
}
#endif