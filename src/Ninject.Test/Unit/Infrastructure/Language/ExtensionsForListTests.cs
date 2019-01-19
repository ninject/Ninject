using Ninject.Infrastructure.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ninject.Tests.Unit.Infrastructure.Language
{
    public class ExtensionsForListTests
    {
        private Random _random;

        public ExtensionsForListTests()
        {
            _random = new Random();
        }

        [Fact]
        public void Union_IReadOnlyListAndIList_FirstAndSecondEmpty()
        {
            IReadOnlyList<string> first = Array.Empty<string>();
            IList<string> second = Array.Empty<string>();

            var actual = first.Union(second);

            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void Union_IReadOnlyListAndIList_FirstEmpty()
        {
            var secondItem1 = new object();
            var secondItem2 = new object();
            var secondItem3 = new object();

            IReadOnlyList<object> first = Array.Empty<object>();
            IList<object> second = new[] { secondItem1, secondItem2, secondItem3, secondItem2 };

            var actual = first.Union(second).ToArray();

            Assert.NotNull(actual);
            Assert.Equal(3, actual.Length);
            Assert.Same(secondItem1, actual[0]);
            Assert.Same(secondItem2, actual[1]);
            Assert.Same(secondItem3, actual[2]);
        }

        [Fact]
        public void Union_IReadOnlyListAndIList_SecondEmpty()
        {
            var firstItem1 = new object();
            var firstItem2 = new object();

            IReadOnlyList<object> first = new[] { firstItem1, firstItem2, firstItem1 };
            IList<object> second = Array.Empty<object>();

            var actual = first.Union(second).ToArray(); ;

            Assert.NotNull(actual);
            Assert.Equal(2, actual.Length);
            Assert.Same(firstItem1, actual[0]);
            Assert.Same(firstItem2, actual[1]);
        }

        [Fact]
        public void Union_IReadOnlyListAndIList_FirstContainsDuplicates()
        {
            var firstItem1 = new object();
            var firstItem2 = new object();
            var firstItem3 = new object();
            var secondItem1 = new object();
            var secondItem2 = new object();

            IReadOnlyList<object> first = new[] { firstItem1, firstItem2, firstItem1, firstItem3 };
            IList<object> second = new[] { secondItem1, secondItem2 };

            var actual = first.Union(second).ToArray(); ;

            Assert.NotNull(actual);
            Assert.Equal(5, actual.Length);
            Assert.Same(firstItem1, actual[0]);
            Assert.Same(firstItem2, actual[1]);
            Assert.Same(firstItem3, actual[2]);
            Assert.Same(secondItem1, actual[3]);
            Assert.Same(secondItem2, actual[4]);
        }

        [Fact]
        public void Union_IReadOnlyListAndIList_SecondContainsDuplicates()
        {
            var firstItem1 = new object();
            var firstItem2 = new object();
            var secondItem1 = new object();
            var secondItem2 = new object();
            var secondItem3 = new object();

            IReadOnlyList<object> first = new[] { firstItem1, firstItem2 };
            IList<object> second = new[] { secondItem1, secondItem2, secondItem1, secondItem3 };

            var actual = first.Union(second).ToArray(); ;

            Assert.NotNull(actual);
            Assert.Equal(5, actual.Length);
            Assert.Same(firstItem1, actual[0]);
            Assert.Same(firstItem2, actual[1]);
            Assert.Same(secondItem1, actual[2]);
            Assert.Same(secondItem2, actual[3]);
            Assert.Same(secondItem3, actual[4]);
        }

        [Fact]
        public void Union_IReadOnlyListAndIList_DuplicatesInUnion()
        {
            var firstItem1 = new object();
            var firstItem2 = new object();
            var secondItem1 = new object();
            var secondItem2 = new object();
            var secondItem3 = new object();

            IReadOnlyList<object> first = new[] { firstItem1, firstItem2, firstItem2};
            IList<object> second = new[] { secondItem1, secondItem2, firstItem1, secondItem3 };

            var actual = first.Union(second).ToArray(); ;

            Assert.NotNull(actual);
            Assert.Equal(5, actual.Length);
            Assert.Same(firstItem1, actual[0]);
            Assert.Same(firstItem2, actual[1]);
            Assert.Same(secondItem1, actual[2]);
            Assert.Same(secondItem2, actual[3]);
            Assert.Same(secondItem3, actual[4]);
        }

        [Fact]
        public void Concat_IReadOnlyListAndIList_FirstAndSecondEmpty()
        {
            IReadOnlyList<string> first = Array.Empty<string>();
            IList<string> second = Array.Empty<string>();

            var actual = first.Concat(second);

            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void Concat_IReadOnlyListAndIList_FirstEmpty()
        {
            var secondItem1 = _random.Next(1, 10);
            var secondItem2 = _random.Next(11, 20);

            IReadOnlyList<int> first = Array.Empty<int>();
            IList<int> second = new[] { secondItem1, secondItem2 };

            var actual = first.Concat(second);

            Assert.NotNull(actual);
            Assert.Equal(second, actual);
            Assert.NotSame(second, actual);
        }

        [Fact]
        public void Concat_IReadOnlyListAndIList_SecondEmpty()
        {
            var firstItem1 = _random.Next(1, 10);
            var firstItem2 = _random.Next(11, 20);

            IReadOnlyList<int> first = new[] { firstItem1, firstItem2 };
            IList<int> second = Array.Empty<int>();

            var actual = first.Concat(second);

            Assert.NotNull(actual);
            Assert.Equal(first, actual);
            Assert.Same(first, actual);
        }

        [Fact]
        public void Concat_IReadOnlyListAndIList_FirstContainsDuplicates()
        {
            var firstItem1 = _random.Next(1, 10);
            var firstItem2 = _random.Next(11, 20);
            var firstItem3 = _random.Next(21, 30);
            var secondItem1 = _random.Next(31, 40);
            var secondItem2 = _random.Next(41, 50);

            IReadOnlyList<int> first = new[] { firstItem1, firstItem2, firstItem1, firstItem3 };
            IList<int> second = new[] { secondItem1, secondItem2 };

            var actual = first.Concat(second);

            Assert.NotNull(actual);
            Assert.Equal(6, actual.Count);
            Assert.Equal(firstItem1, actual[0]);
            Assert.Equal(firstItem2, actual[1]);
            Assert.Equal(firstItem1, actual[2]);
            Assert.Equal(firstItem3, actual[3]);
            Assert.Equal(secondItem1, actual[4]);
            Assert.Equal(secondItem2, actual[5]);
        }

        [Fact]
        public void Concat_IReadOnlyListAndIList_SecondContainsDuplicates()
        {
            var firstItem1 = _random.Next(1, 10);
            var firstItem2 = _random.Next(11, 20);
            var secondItem1 = _random.Next(21, 40);
            var secondItem2 = _random.Next(31, 40);
            var secondItem3 = _random.Next(41, 50);

            IReadOnlyList<int> first = new[] { firstItem1, firstItem2 };
            IList<int> second = new[] { secondItem1, secondItem2, secondItem1, secondItem3 };

            var actual = first.Concat(second);

            Assert.NotNull(actual);
            Assert.Equal(6, actual.Count);
            Assert.Equal(firstItem1, actual[0]);
            Assert.Equal(firstItem2, actual[1]);
            Assert.Equal(secondItem1, actual[2]);
            Assert.Equal(secondItem2, actual[3]);
            Assert.Equal(secondItem1, actual[4]);
        }

        [Fact]
        public void Concat_IReadOnlyListAndIList_DuplicatesInUnion()
        {
            var firstItem1 = _random.Next(1, 10);
            var firstItem2 = _random.Next(11, 20);
            var secondItem1 = _random.Next(21, 40);
            var secondItem2 = _random.Next(31, 40);
            var secondItem3 = _random.Next(41, 50);

            IReadOnlyList<int> first = new[] { firstItem1, firstItem2 };
            IList<int> second = new[] { secondItem1, secondItem2, firstItem1, secondItem3 };

            var actual = first.Concat(second);

            Assert.NotNull(actual);
            Assert.Equal(6, actual.Count);
            Assert.Equal(firstItem1, actual[0]);
            Assert.Equal(firstItem2, actual[1]);
            Assert.Equal(secondItem1, actual[2]);
            Assert.Equal(secondItem2, actual[3]);
            Assert.Equal(firstItem1, actual[4]);
            Assert.Equal(secondItem3, actual[5]);
        }
    }
}
