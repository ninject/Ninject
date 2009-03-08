using System;
using Moq;
using Ninject.Planning.Bindings;
using Xunit;
using Xunit.Should;

namespace Ninject.Tests.Unit.NamedAttributeTests
{
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
			attribute.Matches(metadataMock.Object).ShouldBeTrue();
		}

		[Fact]
		public void ReturnsFalseIfTheNameDoesNotMatch()
		{
			metadataMock.SetupGet(x => x.Name).Returns("bar");
			attribute.Matches(metadataMock.Object).ShouldBeFalse();
		}
	}
}