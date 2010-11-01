namespace Ninject.Tests.Unit.CallbackProviderTests
{
    using System;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Tests.Fakes;
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

    public class CallbackProviderContext
    {
        protected CallbackProvider<Sword> provider;
        protected Mock<IContext> contextMock;

        public CallbackProviderContext()
        {
            this.SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.contextMock = new Mock<IContext>();
        }
    }

    [TestClass]
    public class WhenCreateIsCalled : CallbackProviderContext
    {
        [Fact]
        public void ProviderInvokesCallbackToRetrieveValue()
        {
            var sword = new Sword();
            provider = new CallbackProvider<Sword>(c => sword);

            var result = provider.Create(contextMock.Object);

            result.ShouldBeSameAs(sword);
        }
    }
}