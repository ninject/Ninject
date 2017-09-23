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
    }
}