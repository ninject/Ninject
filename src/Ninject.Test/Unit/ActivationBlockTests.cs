#if !NO_MOQ
namespace Ninject.Tests.Unit.ActivationBlockTests
{
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Blocks;
    using Ninject.Syntax;
    using Xunit;

    public class ActivationBlockContext
    {
        protected ActivationBlock block;
        protected Mock<IResolutionRoot> parentMock;
        protected Mock<IRequest> requestMock;

        public ActivationBlockContext()
        {
            this.parentMock = new Mock<IResolutionRoot>();
            this.requestMock = new Mock<IRequest>();
            this.block = new ActivationBlock(this.parentMock.Object);
        }
    }

    public class WhenCanResolveIsCalled : ActivationBlockContext
    {
        [Fact]
        public void DelegatesCallToParent()
        {
            IRequest request = requestMock.Object;
            block.CanResolve(request);
            parentMock.Verify(x => x.CanResolve(request));
        }
    
        [Fact]
        public void DelegatesCallToParent2()
        {
            IRequest request = requestMock.Object;
            block.CanResolve(request, true);
            parentMock.Verify(x => x.CanResolve(request, true));
        }
    }

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
#endif