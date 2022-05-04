using BenchmarkDotNet.Attributes;
using Ninject.Injection;
using Ninject.Planning.Directives;
using Ninject.Tests.Fakes;
using System;
using System.Reflection;

namespace Ninject.Benchmarks.Planning.Directives
{
    [MemoryDiagnoser]
    public class ConstructorInjectionDirectiveBenchmark
    {
        private ConstructorInfo _zeroArgumentConstructor;
        private ConstructorInfo _twoArgumentConstructor;
        private ConstructorInjector _zeroArgumentIinjector;
        private ConstructorInjector _twoArgumentIinjector;

        public ConstructorInjectionDirectiveBenchmark()
        {
            _zeroArgumentConstructor = typeof(NinjaBarracks).GetConstructor(new Type[0]);
            _twoArgumentConstructor = typeof(NinjaBarracks).GetConstructor(new[] { typeof(IWarrior), typeof(IWeapon) });

            var injectorFactory = new DynamicMethodInjectorFactory();

            _zeroArgumentIinjector = injectorFactory.Create(_zeroArgumentConstructor);
            _twoArgumentIinjector = injectorFactory.Create(_twoArgumentConstructor);
        }

        [Benchmark]
        public void ZeroArguments()
        {
            var directive = new ConstructorInjectionDirective(_zeroArgumentConstructor, _zeroArgumentIinjector);
            if (directive == null)
                throw new Exception();
        }

        [Benchmark]
        public void TwoArguments()
        {
            var directive = new ConstructorInjectionDirective(_twoArgumentConstructor, _twoArgumentIinjector);
            if (directive == null)
                throw new Exception();
        }
    }
}
