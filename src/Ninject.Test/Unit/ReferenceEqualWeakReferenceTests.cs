namespace Ninject.Tests.Unit
{
    using FluentAssertions;
    using Ninject.Infrastructure;
    using Xunit;

#if MSTEST
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
#endif
    public class ReferenceEqualWeakReferenceTests
    {
#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void TwoReferencesReferencingTheSameObjectAreEqual()
        {
            var instance = new object();
            var ref1 = new ReferenceEqualWeakReference(instance);
            var ref2 = new ReferenceEqualWeakReference(instance);

            ref1.Equals(ref2).Should().BeTrue();
            ref1.GetHashCode().Should().Be(ref2.GetHashCode());
        }

#if !MSTEST 
        [Fact]
#else
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
#endif
        public void ReferencesIsEqualToTheInstanceItIsReferingTo()
        {
            var instance = new object();
            var reference = new ReferenceEqualWeakReference(instance);

            reference.Equals(instance).Should().BeTrue();
            reference.GetHashCode().Should().Be(instance.GetHashCode());
        }
    }
}