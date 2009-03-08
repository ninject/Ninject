using System;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Providers;
using Ninject.Tests.Fakes;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.ConstantProviderTests
{
	public class ConstantProviderContext
	{
		protected ConstantProvider<Sword> provider;
		protected readonly Mock<IContext> contextMock;

		public ConstantProviderContext()
		{
			contextMock = new Mock<IContext>();
		}
	}

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