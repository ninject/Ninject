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
            IRequest request = this.requestMock.Object;
            this.block.CanResolve(request);
            this.parentMock.Verify(x => x.CanResolve(request));
        }
    
        [Fact]
        public void DelegatesCallToParent2()
        {
            IRequest request = this.requestMock.Object;
            this.block.CanResolve(request, true);
            this.parentMock.Verify(x => x.CanResolve(request, true));
        }
    }

    public class WhenResolveIsCalledWithRequestObject : ActivationBlockContext
    {
        [Fact]
        public void DelegatesCallToParent()
        {
            IRequest request = this.requestMock.Object;
            this.block.Resolve(request);
            this.parentMock.Verify(x => x.Resolve(request));
        }
    }
}