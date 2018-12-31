using System.Collections.Generic;
using System.Linq;
using Xunit;
using Ninject.Infrastructure.Language;
using System;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Ninject.Test.Unit.Infrastructure.Language
{
    public class ExtensionsForDictionaryTests
    {
        [Fact]
        public void Clone_Dictionary_ShouldMakeCopyOfValues()
        {
            const string valueA = "A";
            const string valueB = "B";
            var list1 = new List<string> { valueA, valueB };
            var list2 = new List<string> { valueB, valueA };
            var list3 = new List<string>();

            var source = new Dictionary<int, ICollection<string>>
                {
                    { 1, list1 },
                    { 2, list2 },
                    { 3, list3 },
                };

            var clone = source.Clone();

            Assert.NotNull(clone);
            Assert.Equal(source.Count, clone.Count);

            Assert.True(clone.ContainsKey(1));
            var clonedCollection1 = clone[1];
            Assert.NotSame(list1, clonedCollection1);
            Assert.Equal(2, clonedCollection1.Count);
            var clonedList1 = clonedCollection1.ToList();
            Assert.Same(valueA, clonedList1[0]);
            Assert.Same(valueB, clonedList1[1]);

            Assert.True(clone.ContainsKey(2));
            var clonedCollection2 = clone[2];
            Assert.NotSame(list2, clonedCollection2);
            Assert.Equal(2, clonedCollection2.Count);
            var clonedList2 = clonedCollection2.ToList();
            Assert.Same(valueB, clonedList2[0]);
            Assert.Same(valueA, clonedList2[1]);

            Assert.True(clone.ContainsKey(3));
            var clonedCollection3 = clone[3];
            Assert.NotSame(list3, clonedCollection3);
            Assert.Equal(0, clonedCollection3.Count);
        }

        [Fact]
        public void Clone_Dictionary_ShouldThrowNullReferenceExceptionWhenDictionaryIsNull()
        {
            Dictionary<string, ICollection<int>> dictionary = null;

            Assert.Throws<NullReferenceException>(() => dictionary.Clone());
        }

        [Fact]
        public void Clone_DictionaryAndComparer_ShouldMakeCopyOfValues()
        {
            const string valueA = "A";
            const string valueB = "B";
            var list1 = new List<string> { valueA, valueB };
            var list2 = new List<string> { valueB, valueA };
            var list3 = new List<string>();

            var source = new Dictionary<int, ICollection<string>>
                {
                    { 1, list1 },
                    { 2, list2 },
                    { 3, list3 },
                };

            var clone = source.Clone();

            Assert.NotNull(clone);
            Assert.Equal(source.Count, clone.Count);

            Assert.True(clone.ContainsKey(1));
            var clonedCollection1 = clone[1];
            Assert.NotSame(list1, clonedCollection1);
            Assert.Equal(2, clonedCollection1.Count);
            var clonedList1 = clonedCollection1.ToList();
            Assert.Same(valueA, clonedList1[0]);
            Assert.Same(valueB, clonedList1[1]);

            Assert.True(clone.ContainsKey(2));
            var clonedCollection2 = clone[2];
            Assert.NotSame(list2, clonedCollection2);
            Assert.Equal(2, clonedCollection2.Count);
            var clonedList2 = clonedCollection2.ToList();
            Assert.Same(valueB, clonedList2[0]);
            Assert.Same(valueA, clonedList2[1]);

            Assert.True(clone.ContainsKey(3));
            var clonedCollection3 = clone[3];
            Assert.NotSame(list3, clonedCollection3);
            Assert.Equal(0, clonedCollection3.Count);
        }

        [Fact]
        public void Clone_DictionaryAndComparer_ShouldUseComparer()
        {
            var valueA = new List<string> { "ValueA", "Valuea" };
            var valueB = new List<string> { "Valueb", "ValueB" };

            var source = new Dictionary<string, ICollection<string>>
                {
                    { "A", valueA },
                    { "B", valueB },
                };

            var clone = source.Clone(new OrdinalIgnoreCaseComparer());

            Assert.NotNull(clone);
            Assert.Equal(source.Count, clone.Count);

            Assert.True(clone.ContainsKey("b"));
            Assert.Equal(valueA, clone["a"]);

            var newValueA = new List<string> { "newValueA" };
            clone["a"] = newValueA;

            Assert.Equal(2, clone.Count);
            Assert.Same(newValueA, clone["A"]);
            Assert.Same(newValueA, clone["a"]);
        }

        [Fact]
        public void Clone_DictionaryAndComparer_ShouldUseDefaultEqualityComparerWhenComparerIsNull()
        {
            var valueA = new List<string> { "ValueA", "Valuea" };
            var valueB = new List<string> { "Valueb", "ValueB" };

            var source = new Dictionary<string, ICollection<string>>
                {
                    { "A", valueA },
                    { "B", valueB },
                };

            var clone = source.Clone(null);

            Assert.NotNull(clone);
            Assert.Equal(source.Count, clone.Count);

            Assert.False(clone.ContainsKey("b"));

            var newValueA = new List<string> { "newValueA" };
            clone["a"] = newValueA;

            Assert.Equal(3, clone.Count);
            Assert.Equal(valueA, clone["A"]);
            Assert.Same(newValueA, clone["a"]);
        }

        [Fact]
        public void Clone_DictionaryAndComparer_ShouldThrowNullReferenceExceptionWhenDictionaryIsNull()
        {
            Dictionary<string, ICollection<int>> dictionary = null;

            Assert.Throws<NullReferenceException>(() => dictionary.Clone());
        }

        private class OrdinalIgnoreCaseComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.ToUpperInvariant().GetHashCode();
            }
        }
    }
}
