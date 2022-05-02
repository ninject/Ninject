namespace Ninject.Tests.Unit
{
    using System;
    using FluentAssertions;
    using Ninject.Parameters;
    using Ninject.Tests.Fakes;
    using Xunit;

    public class TypeMatchingConstructorArgumentTest : IDisposable
    {
        [Fact]
        public void InstancesAreEqualIfTypeIsEqual()
        {
            var firstInstance = new TypeMatchingConstructorArgument(typeof(Samurai), (context, target) => null);
            var secondInstance = new TypeMatchingConstructorArgument(typeof(Samurai), (context, target) => null);

            var result = firstInstance.Equals(secondInstance);

            result.Should().BeTrue();
        }

        [Fact]
        public void InstancesAreNotEqual()
        {
            var firstInstance = new TypeMatchingConstructorArgument(typeof(Samurai), (context, target) => null);
            var secondInstance = new TypeMatchingConstructorArgument(typeof(Ninja), (context, target) => null);

            var result = firstInstance.Equals(secondInstance);

            result.Should().BeFalse();
        }

        public void Dispose()
        {
        }
    }
}