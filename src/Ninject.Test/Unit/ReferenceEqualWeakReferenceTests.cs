namespace Ninject.Tests.Unit
{
    using FluentAssertions;
    using Ninject.Infrastructure;
    using Xunit;

    public class ReferenceEqualWeakReferenceTests
    {
        [Fact]
        public void TwoReferencesReferencingTheSameObjectAreEqual()
        {
            var instance = new object();
            var ref1 = new ReferenceEqualWeakReference(instance);
            var ref2 = new ReferenceEqualWeakReference(instance);

            ref1.Equals(ref2).Should().BeTrue();
            ref1.GetHashCode().Should().Be(ref2.GetHashCode());
        }

        [Fact]
        public void ReferencesIsEqualToTheInstanceItIsReferingTo()
        {
            var instance = new object();
            var reference = new ReferenceEqualWeakReference(instance);

            reference.Equals(instance).Should().BeTrue();
            reference.GetHashCode().Should().Be(instance.GetHashCode());
        }
    }
}