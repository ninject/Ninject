using BenchmarkDotNet.Attributes;
using Ninject.Parameters;
using System.Collections.Generic;

namespace Ninject.Benchmarks.Parameters
{
    [MemoryDiagnoser]
    public class ExtensionsForParameterBenchmark
    {
        private IReadOnlyList<IParameter> _parametersEmpty;
        private IReadOnlyList<IParameter> _allParametersHaveShouldInheritSetToFalse;
        private IReadOnlyList<IParameter> _allParametersHaveShouldInheritSetToTrue;
        private IReadOnlyList<IParameter> _mixOfParametersWithShouldInheritSetToTrueAndFalse;

        public ExtensionsForParameterBenchmark()
        {
            _parametersEmpty = new List<IParameter>();
            _allParametersHaveShouldInheritSetToFalse = new List<IParameter>
                {
                    new ConstructorArgument("foo1", 1, false),
                    new ConstructorArgument("foo2", 2, false),
                    new ConstructorArgument("foo3", 3, false),
                    new ConstructorArgument("foo4", 4, false),
                    new ConstructorArgument("foo5", 5, false)
                };
            _allParametersHaveShouldInheritSetToTrue = new List<IParameter>
                {
                    new ConstructorArgument("foo1", 1, true),
                    new ConstructorArgument("foo2", 2, true),
                    new ConstructorArgument("foo3", 3, true),
                    new ConstructorArgument("foo4", 4, true),
                    new ConstructorArgument("foo5", 5, true)
                };
            _mixOfParametersWithShouldInheritSetToTrueAndFalse = new List<IParameter>
                {
                    new ConstructorArgument("foo1", 1, true),
                    new ConstructorArgument("foo2", 2, false),
                    new ConstructorArgument("foo3", 3, true),
                    new ConstructorArgument("foo4", 4, false),
                    new ConstructorArgument("foo5", 5, true)
                };
        }

        [Benchmark]
        public void GetShouldInheritParameters_ParametersIsEmpty()
        {
            _parametersEmpty.GetShouldInheritParameters();
        }

        [Benchmark]
        public void GetShouldInheritParameters_AllParametersHaveShouldInheritSetToFalse()
        {
            _allParametersHaveShouldInheritSetToFalse.GetShouldInheritParameters();
        }

        [Benchmark]
        public void GetShouldInheritParameters_AllParametersHaveShouldInheritSetToTrue()
        {
            _allParametersHaveShouldInheritSetToTrue.GetShouldInheritParameters();
        }

        [Benchmark]
        public void GetShouldInheritParameters_MixOfParametersWithShouldInheritSetToTrueAndFalse()
        {
            _mixOfParametersWithShouldInheritSetToTrueAndFalse.GetShouldInheritParameters();
        }
    }
}
