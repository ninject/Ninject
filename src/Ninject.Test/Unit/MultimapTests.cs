namespace Ninject.Tests.Unit
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Ninject.Infrastructure;

    using Xunit;

    public class MultimapTests
    {
        [Fact]
        public void CanAddMultipleValuesToSameKey()
        {
            var testee = new Multimap<int, int>();

            testee.Add(1, 1);
            testee.Add(1, 2);

            testee[1].Should().Contain(1);
            testee[1].Should().Contain(2);
        }

        [Fact]
        public void TryGetValues_WhenKeyExists_ReturnsValues()
        {
            var testee = new Multimap<int, int>();

            testee.Add(1, 1);
            testee.Add(1, 2);

            IEnumerable<int> values;
            var result = testee.TryGetValues(1, out values);

            result.Should().BeTrue();
            values.Should().Contain(1);
            values.Should().Contain(2);
        }

        [Fact]
        public void TryGetValues_WhenKeyNotExists_ReturnsFalse()
        {
            var testee = new Multimap<int, int>();

            IEnumerable<int> values;
            var result = testee.TryGetValues(1, out values);

            result.Should().BeFalse();
        }

    }
}