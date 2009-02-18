using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Scope;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using Ninject.Tests.Fakes;
using Xunit;

namespace Ninject.Tests.Unit.ActivationScopeTests
{
	public class ActivationScopeContext
	{
		protected readonly ActivationScope scope;
		protected readonly Mock<IResolutionRoot> parentMock;
		protected readonly Mock<IRequest> requestMock;

		public ActivationScopeContext()
		{
			parentMock = new Mock<IResolutionRoot>();
			requestMock = new Mock<IRequest>();
			scope = new ActivationScope(parentMock.Object);
		}
	}

	public class WhenCanResolveIsCalled : ActivationScopeContext
	{
		[Fact]
		public void ScopeDelegatesCallToParent()
		{
			IRequest request = requestMock.Object;
			scope.CanResolve(request);
			parentMock.Verify(x => x.CanResolve(request));
		}
	}

	public class WhenResolveIsCalledWithRequestObject : ActivationScopeContext
	{
		[Fact]
		public void ScopeDelegatesCallToParent()
		{
			IRequest request = requestMock.Object;
			scope.Resolve(request);
			parentMock.Verify(x => x.Resolve(request));
		}
	}

	public class WhenResolveIsCalledWithServiceType : ActivationScopeContext
	{
		[Fact]
		public void ScopeCreatesRequestAndDelegatesCallToParent()
		{
			scope.Resolve(typeof(IWeapon), null, new IParameter[0], false);
			parentMock.Verify(x => x.Resolve(It.Is<Request>(r => r.Service == typeof(IWeapon) && r.Constraint == null && r.Parameters.Count == 0)));
		}

		[Fact]
		public void ScopeCreatesRequestWithItselfAsScope()
		{
			scope.Resolve(typeof(IWeapon), null, new IParameter[0], false);
			parentMock.Verify(x => x.Resolve(It.Is<Request>(r => ReferenceEquals(r.ScopeCallback(), scope))));
		}
	}
}