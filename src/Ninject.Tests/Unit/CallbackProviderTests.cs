using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.CallbackProviderTests
{
	public class CallbackProviderContext
	{
		protected CallbackProvider<Sword> provider;
		protected readonly Mock<IContext> contextMock;

		public CallbackProviderContext()
		{
			contextMock = new Mock<IContext>();
		}
	}

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