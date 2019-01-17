using System;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;

using Ninject.Infrastructure.Language;

namespace Ninject.Benchmarks.Infrastructure.Language
{
    [MemoryDiagnoser]
    public class ExtensionsForListBenchmark
    {
        private IReadOnlyList<object> _readOnlyListEmpty;
        private List<object> _listEmpty;
        private IReadOnlyList<object> _readOnlyListNotEmpty;
        private List<object> _listNotEmpty;

        public ExtensionsForListBenchmark()
        {
            var item1 = new object();
            var item2 = new object();
            var item3 = new object();
            var item4 = new object();

            _readOnlyListEmpty = Array.Empty<object>();
            _listEmpty = new List<object>();

            _readOnlyListNotEmpty = new[] { item1, item2, item1 };
            _listNotEmpty = new List<object> { item3, item4, item3 };
        }

        [Benchmark]
        public void Concat_IReadOnlyListAndIList_FirstAndSecondEmpty()
        {
            _readOnlyListEmpty.Concat(_listEmpty);
        }

        [Benchmark]
        public void Concat_IReadOnlyListAndIList_FirstEmpty()
        {
            _readOnlyListEmpty.Concat(_listNotEmpty);
        }

        [Benchmark]
        public void Concat_IReadOnlyListAndIList_SecondEmpty()
        {
            _readOnlyListNotEmpty.Concat(_listEmpty);
        }

        [Benchmark]
        public void Concat_IReadOnlyListAndIList_FirstAndSecondAreNotEmpty()
        {
            _readOnlyListNotEmpty.Concat(_listNotEmpty);
        }

        [Benchmark]
        public void Union_IReadOnlyListAndIList_FirstAndSecondEmpty()
        {
            _readOnlyListEmpty.Union(_listEmpty);
        }

        [Benchmark]
        public void Union_IReadOnlyListAndIList_FirstEmpty()
        {
            _readOnlyListEmpty.Union(_listNotEmpty);
        }

        [Benchmark]
        public void Union_IReadOnlyListAndIList_SecondEmpty()
        {
            _readOnlyListNotEmpty.Union(_listEmpty);
        }

        [Benchmark]
        public void Union_IReadOnlyListAndIList_FirstAndSecondAreNotEmpty()
        {
            _readOnlyListNotEmpty.Union(_listNotEmpty);
        }

        [Benchmark]
        public void LinqUnion_IReadOnlyListAndIList_FirstAndSecondEmpty()
        {
            System.Linq.Enumerable.ToList(System.Linq.Enumerable.Union(_readOnlyListEmpty, _listEmpty));
        }

        [Benchmark]
        public void LinqUnion_IReadOnlyListAndIList_FirstEmpty()
        {
            System.Linq.Enumerable.ToList(System.Linq.Enumerable.Union(_readOnlyListEmpty, _listNotEmpty));
        }

        [Benchmark]
        public void LinqUnion_IReadOnlyListAndIList_SecondEmpty()
        {
            System.Linq.Enumerable.ToList(System.Linq.Enumerable.Union(_readOnlyListNotEmpty, _listEmpty));
        }

        [Benchmark]
        public void LinqUnion_IReadOnlyListAndIList_FirstAndSecondAreNotEmpty()
        {
            System.Linq.Enumerable.ToList(System.Linq.Enumerable.Union(_readOnlyListNotEmpty, _listNotEmpty));
        }
    }
}
