namespace Ninject.Tests.Unit
{
    using System;
    using Moq;
    using Ninject.Activation.Caching;
#if SILVERLIGHT
#if SILVERLIGHT_MSTEST
    using MsTest.Should;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Fact = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#else
    using UnitDriven;
    using UnitDriven.Should;
    using Fact = UnitDriven.TestMethodAttribute;
#endif
#else
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;
#endif

    [TestClass]
    public class ActivationCacheTests
    {
        private ActivationCache testee;

        public ActivationCacheTests()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
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