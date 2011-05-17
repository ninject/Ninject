#if !NO_MOQ
using Moq;
using Ninject.Planning.Bindings;
using Xunit;

namespace Ninject.Tests.Unit.NamedAttributeTests
{
    using FluentAssertions;

    public class NamedAttributeContext
    {
        protected readonly NamedAttribute attribute;
        protected readonly Mock<IBindingMetadata> metadataMock;

        public NamedAttributeContext()
        {
            attribute = new NamedAttribute("foo");
            metadataMock = new Mock<IBindingMetadata>();
        }
    }

    public class WhenMatchesIsCalled : NamedAttributeContext
    {
        [Fact]
        public void ReturnsTrueIfTheNameMatches()
        {
            metadataMock.SetupGet(x => x.Name).Returns("foo");
            attribute.Matches(metadataMock.Object).Should().BeTrue();
        }

        [Fact]
        public void ReturnsFalseIfTheNameDoesNotMatch()
        {
            metadataMock.SetupGet(x => x.Name).Returns("bar");
            attribute.Matches(metadataMock.Object).Should().BeFalse();
        }
    }
}
#endif