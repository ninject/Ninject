namespace Ninject.Tests.Unit
{
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;

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
    public class ActivationCacheStrategyTest
    {
        private ActivationCacheStrategy testee;

        private Mock<IActivationCache> activationCacheMock;

        private INinjectSettings settings;

        public ActivationCacheStrategyTest()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.activationCacheMock = new Mock<IActivationCache>();
            this.settings = new NinjectSettings();
            this.testee = new ActivationCacheStrategy(this.activationCacheMock.Object) { Settings = this.settings };
        }

        [Fact]
        public void InstanceActivationsAreCachedAtActivation()
        {
            var instance  = new object();
            var contextMock = new Mock<IContext>();
            
            this.testee.Activate(contextMock.Object, new InstanceReference { Instance = instance });

            this.activationCacheMock.Verify(activationCache => activationCache.AddActivatedInstance(instance));
        }

        [Fact]
        public void InstanceActivationsAreNotCachedAtActivationWhenDisabled()
        {
            var instance = new object();
            var contextMock = new Mock<IContext>();
            this.settings.ActivationCacheDisabled = true;

            this.testee.Activate(contextMock.Object, new InstanceReference { Instance = instance });

            this.activationCacheMock.Verify(activationCache => activationCache.AddActivatedInstance(instance));
        }

        [Fact]
        public void InstanceDeactivationsAreCachedAtDeactivation()
        {
            var instance = new object();
            var contextMock = new Mock<IContext>();

            this.testee.Deactivate(contextMock.Object, new InstanceReference { Instance = instance });

            this.activationCacheMock.Verify(activationCache => activationCache.AddDeactivatedInstance(instance));
        }
        
        [Fact]
        public void InstanceDeactivationsAreNotCachedAtDeactivationWhenDisabled()
        {
            var instance = new object();
            var contextMock = new Mock<IContext>();

            this.settings.ActivationCacheDisabled = true;
            this.testee.Deactivate(contextMock.Object, new InstanceReference { Instance = instance });

            this.activationCacheMock.Verify(activationCache => activationCache.AddDeactivatedInstance(instance));
        }
    }
}