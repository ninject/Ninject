#if !NO_MOQ
namespace Ninject.Tests.Unit.ConstantProviderTests
{
    using FluentAssertions;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class ConstantProviderContext
    {
        protected ConstantProvider<Sword> provider;
        protected Mock<IContext> contextMock;

        public ConstantProviderContext()
        {
            this.SetUp();
        }

        public void SetUp()
        {
            this.contextMock = new Mock<IContext>();
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

            result.Should().BeSameAs(sword);
        }
    }
}
#endif