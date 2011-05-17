#if !NO_MOQ
namespace Ninject.Tests.Unit.CallbackProviderTests
{
    using FluentAssertions;
    using Moq;
    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class CallbackProviderContext
    {
        protected CallbackProvider<Sword> provider;
        protected Mock<IContext> contextMock;

        public CallbackProviderContext()
        {
            this.SetUp();
        }

        public void SetUp()
        {
            this.contextMock = new Mock<IContext>();
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

            result.Should().BeSameAs(sword);
        }
    }
}
#endif