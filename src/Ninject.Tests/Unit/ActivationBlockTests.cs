using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Blocks;
using Ninject.Syntax;
using Xunit;

namespace Ninject.Tests.Unit.ActivationBlockTests
{
	public class ActivationBlockContext
	{
		protected readonly ActivationBlock block;
		protected readonly Mock<IResolutionRoot> parentMock;
		protected readonly Mock<IRequest> requestMock;

		public ActivationBlockContext()
		{
			parentMock = new Mock<IResolutionRoot>();
			requestMock = new Mock<IRequest>();
			block = new ActivationBlock(parentMock.Object);
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