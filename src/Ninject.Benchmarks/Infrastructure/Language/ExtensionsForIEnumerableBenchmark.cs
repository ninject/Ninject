using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

using Ninject.Infrastructure.Language;
using Ninject.Tests.Fakes;

namespace Ninject.Benchmarks.Infrastructure.Language
{
    public class ExtensionsForIEnumerableBenchmark
    {
        private IEnumerable _emptyEnumerable;
        private IEnumerable _smallEnumerable;
        private IEnumerable _largeEnumerable;

        public ExtensionsForIEnumerableBenchmark()
        {
            _emptyEnumerable = Enumerable.Empty<string>();
            _smallEnumerable = Enumerable.Range(1, 5).Select(i => i.ToString());
            _largeEnumerable = Enumerable.Range(1, 1000).Select(i => i.ToString());
        }

        [Benchmark]
        public void ToListSlow_Empty()
        {
            var list = ExtensionsForIEnumerable.ToListSlow(_emptyEnumerable, typeof(string));
            foreach (var item in list)
            {
                if (item == null)
                    throw new Exception();
            }
        }

        [Benchmark]
        public void ToListSlow_Small()
        {
            var list = ExtensionsForIEnumerable.ToListSlow(_smallEnumerable, typeof(string));
            foreach (var item in list)
            {
                if (item == null)
                    throw new Exception();
            }
        }

        [Benchmark]
        public void ToListSlow_Large()
        {
            var list = ExtensionsForIEnumerable.ToListSlow(_largeEnumerable, typeof(string));
            foreach (var item in list)
            {
                if (item == null)
                    throw new Exception();
            }
        }
    }
}
