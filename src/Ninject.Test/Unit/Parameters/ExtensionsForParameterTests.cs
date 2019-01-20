using Ninject.Parameters;
using System;
using System.Collections.Generic;
using Xunit;

namespace Ninject.Tests.Unit.Parameters
{
    public class ExtensionsForParameterTests
    {
        [Fact]
        public void GetShouldInheritParameters_ShouldThrowNullReferenceExceptionWhenParametersIsNull()
        {
            const IReadOnlyList<IParameter> parameters = null;

            Assert.Throws<NullReferenceException>(() => parameters.GetShouldInheritParameters());
        }

        [Fact]
        public void GetShouldInheritParameters_ShouldFilterOutParameterWithShouldInheritSetToFalse()
        {
            IReadOnlyList<IParameter> parameters = new List<IParameter>
                {
                    new ConstructorArgument("boo", 1, false),
                    new ConstructorArgument("foo", 5, true),
                    new ConstructorArgument("foo", 5, false),
                    new Parameter("bar", "foo", true),
                    new Parameter("foo", "bar", false)
                };

            var actual = parameters.GetShouldInheritParameters();

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count);
            Assert.DoesNotContain(actual, p => !p.ShouldInherit);
        }

        [Fact]
        public void GetShouldInheritParameters_ShouldReturnEmptyArrayWhenParametersIsEmpty()
        {
            IReadOnlyList<IParameter> parameters = new List<IParameter>();

            var actual = parameters.GetShouldInheritParameters();

            Assert.NotNull(actual);
            Assert.Empty(actual);
            Assert.Same(Array.Empty<IParameter>(), actual);
        }

        [Fact]
        public void GetShouldInheritParameters_ShouldReturnEmptyArrayWhenThereAreNoParametersWithShouldInheritSetToTrue()
        {
            IReadOnlyList<IParameter> parameters = new List<IParameter>
                {
                    new ConstructorArgument("boo", 1, false),
                    new ConstructorArgument("foo", 5, false),
                    new Parameter("foo", "bar", false)
                };

            var actual = parameters.GetShouldInheritParameters();

            Assert.NotNull(actual);
            Assert.Empty(actual);
        }
    }
}
