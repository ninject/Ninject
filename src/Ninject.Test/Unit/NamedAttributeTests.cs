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
            this.attribute = new NamedAttribute("foo");
            this.metadataMock = new Mock<IBindingMetadata>();
        }
    }

    public class WhenMatchesIsCalled : NamedAttributeContext
    {
        [Fact]
        public void ReturnsTrueIfTheNameMatches()
        {
            this.metadataMock.SetupGet(x => x.Name).Returns("foo");
            this.attribute.Matches(this.metadataMock.Object).Should().BeTrue();
        }

        [Fact]
        public void ReturnsFalseIfTheNameDoesNotMatch()
        {
            this.metadataMock.SetupGet(x => x.Name).Returns("bar");
            this.attribute.Matches(this.metadataMock.Object).Should().BeFalse();
        }
    }
}