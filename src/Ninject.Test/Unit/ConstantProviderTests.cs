namespace Ninject.Tests.Unit.ConstantProviderTests
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
    
    public class ConstantProviderContext
    {
        protected ConstantProvider<Sword> provider;
        protected Mock<IContext> contextMock;

        public ConstantProviderContext()
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
    public class WhenCreateIsCalled : ConstantProviderContext
    {
        [Fact]
        public void ProviderReturnsConstantValue()
        {
            var sword = new Sword();
            provider = new ConstantProvider<Sword>(sword);

            var result = provider.Create(contextMock.Object);

            result.ShouldBeSameAs(sword);
        }
    }
}