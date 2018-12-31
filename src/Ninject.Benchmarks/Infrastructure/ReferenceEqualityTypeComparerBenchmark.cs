using BenchmarkDotNet.Attributes;
using Ninject.Infrastructure;
using System;
using System.Collections.Generic;

namespace Ninject.Benchmarks.Infrastructure
{
    public class ReferenceEqualityTypeComparerBenchmark
    {
        private ReferenceEqualityTypeComparer _refEqualityComparer;
        private EqualityComparer<Type> _defaultComparer;
        private Dictionary<Type, string> _refEqualityDictionary;
        private Dictionary<Type, string> _defaultDictionary;
        private Type _bclType;
        private Type _ninjectType;

        public ReferenceEqualityTypeComparerBenchmark()
        {
            _refEqualityComparer = new ReferenceEqualityTypeComparer();
            _defaultComparer = EqualityComparer<Type>.Default;

            _refEqualityDictionary = new Dictionary<Type, string>(_refEqualityComparer);
            _defaultDictionary = new Dictionary<Type, string>();

            foreach (var t in typeof(string).Assembly.GetTypes())
            {
                _refEqualityDictionary[t] = t.FullName;
                _defaultDictionary[t] = t.FullName;
            }

            _bclType = typeof(string);
            _ninjectType = GetType();
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void DictionaryLookup_Match_ReferenceEquality()
        {
            for (var i = 0; i < 1000; i++)
            {
                _refEqualityDictionary.ContainsKey(_bclType);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void DictionaryLookup_Match_Default()
        {
            for (var i = 0; i < 1000; i++)
            {
                _defaultDictionary.ContainsKey(_bclType);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void DictionaryLookup_NoMatch_ReferenceEquality()
        {
            for (var i = 0; i < 1000; i++)
            {
                _refEqualityDictionary.ContainsKey(_ninjectType);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void DictionaryLookup_NoMatch_Default()
        {
            for (var i = 0; i < 1000; i++)
            {
                _defaultDictionary.ContainsKey(_ninjectType);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void GetHashCode_ReferenceEquality()
        {
            for (var i = 0; i < 1000; i++)
            {
                _refEqualityComparer.GetHashCode(_bclType);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void GetHashCode_Default()
        {
            for (var i = 0; i < 1000; i++)
            {
                _defaultComparer.GetHashCode(_bclType);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void EqualsInstancesTheSame_ReferenceEquality()
        {
            for (var i = 0; i < 1000; i++)
            {
                _refEqualityComparer.Equals(_bclType, _bclType);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void EqualsInstancesTheSame_Default()
        {
            for (var i = 0; i < 1000; i++)
            {
                _defaultComparer.Equals(_bclType, _bclType);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void EqualsInstancesNotTheSame_ReferenceEquality()
        {
            for (var i = 0; i < 1000; i++)
            {
                _refEqualityComparer.Equals(_ninjectType, _bclType);
            }
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public void EqualsInstancesNotTheSame_Default()
        {
            for (var i = 0; i < 1000; i++)
            {
                _defaultComparer.Equals(_ninjectType, _bclType);
            }
        }
    }
}
