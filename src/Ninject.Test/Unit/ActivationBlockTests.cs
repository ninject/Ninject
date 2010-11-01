namespace Ninject.Tests.Unit.ActivationBlockTests
{
    using System;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Blocks;
    using Ninject.Syntax;
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
    public class ActivationBlockContext
    {
        protected ActivationBlock block;
        protected Mock<IResolutionRoot> parentMock;
        protected Mock<IRequest> requestMock;

        public ActivationBlockContext()
        {
            SetUp();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.parentMock = new Mock<IResolutionRoot>();
            this.requestMock = new Mock<IRequest>();
            this.block = new ActivationBlock(this.parentMock.Object);
        }
    }

    [TestClass]
    public class WhenCanResolveIsCalled : ActivationBlockContext
    {
        [Fact]
        public void DelegatesCallToParent()
        {
            IRequest request = requestMock.Object;
            block.CanResolve(request);
            parentMock.Verify(x => x.CanResolve(request));
        }
    }

    [TestClass]
    public class WhenResolveIsCalledWithRequestObject : ActivationBlockContext
    {
        [Fact]
        public void DelegatesCallToParent()
        {
            IRequest request = requestMock.Object;
            block.Resolve(request);
            parentMock.Verify(x => x.Resolve(request));
        }
    }
}