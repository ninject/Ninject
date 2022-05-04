using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Components;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Planning.Directives;
using Ninject.Selection.Heuristics;
using Ninject.Tests.Fakes;
using Ninject.Tests.Integration;
using Ninject.Tests.Integration.EnumerableDependenciesTests.Fakes;

namespace Ninject.Benchmarks.Selection.Heuristics
{
    [MemoryDiagnoser]
    public class StandardConstructorScorerBenchmark
    {
        private ConstructorInfo _injectCtor;
        private ConstructorInjectionDirective _injectCtorDirective;
        private ConstructorInfo _defaultCtor;
        private ConstructorInjectionDirective _defaultCtorDirective;
        private ConstructorInfo _knifeCtor;
        private ConstructorInjectionDirective _knifeCtorDirective;
        private ConstructorInfo _knifeDefaultsCtor;
        private ConstructorInjectionDirective _knifeDefaultsCtorDirective;
        private ConstructorInfo _spartanNameAndAgeCtor;
        private ConstructorInjectionDirective _spartanNameAndAgeCtorDirective;
        private ConstructorInfo _spartanHeightAndWeaponCtor;
        private ConstructorInjectionDirective _spartanHeightAndWeaponCtorDirective;
        private ConstructorInfo _barracksCtor;
        private ConstructorInjectionDirective _barracksCtorDirective;
        private ConstructorInfo _monasteryCtor;
        private ConstructorInjectionDirective _monasteryCtorDirective;
        private ConstructorInfo _enumerableCtor;
        private ConstructorInjectionDirective _enumerableCtorDirective;
        private StandardConstructorScorer _standardConstructorScorer;
        private IInjectorFactory _injectorFactory;
        private Context _contextWithParams;
        private Context _contextWithoutParams;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var ninjectSettings = new NinjectSettings { LoadExtensions = false };

            var kernel = new StandardKernel(ninjectSettings);
            kernel.Bind<IWarrior>().To<SpecialNinja>().WhenInjectedExactlyInto<NinjaBarracks>();
            kernel.Bind<IWarrior>().To<Samurai>().WhenInjectedExactlyInto<Barracks>();
            kernel.Bind<IWarrior>().To<FootSoldier>().WhenInjectedExactlyInto<Barracks>();
            kernel.Bind<IWeapon>().To<Shuriken>().WhenInjectedExactlyInto<Barracks>();
            kernel.Bind<IWeapon>().To<ShortSword>().WhenInjectedExactlyInto<Spartan>();

            _injectorFactory = new DynamicMethodInjectorFactory();

            _contextWithParams = CreateContext(kernel,
                                               new List<IParameter>
                                                    {
                                                        new ConstructorArgument("height", 34),
                                                        new PropertyValue("name", "cutter"),
                                                        new ConstructorArgument("width", 17),
                                                        new ConstructorArgument("location", "Biutiful")
                                                    },
                                               typeof(StandardConstructorScorerBenchmark));

            _contextWithoutParams = CreateContext(kernel,
                                                  Array.Empty<IParameter>(),
                                                  typeof(StandardConstructorScorerBenchmark));

            _injectCtor = typeof(NinjaBarracks).GetConstructor(new[] { typeof(IWarrior), typeof(IWeapon) });
            _injectCtorDirective = new ConstructorInjectionDirective(_injectCtor, _injectorFactory.Create(_injectCtor));

            _defaultCtor = typeof(NinjaBarracks).GetConstructor(new Type[0]);
            _defaultCtorDirective = new ConstructorInjectionDirective(_defaultCtor, _injectorFactory.Create(_defaultCtor));

            _knifeCtor = typeof(Knife).GetConstructor(new[] { typeof(int), typeof(int) });
            _knifeCtorDirective = new ConstructorInjectionDirective(_knifeCtor, _injectorFactory.Create(_knifeCtor));

            _knifeDefaultsCtor = typeof(Knife).GetConstructor(new[] { typeof(bool), typeof(string) });
            _knifeDefaultsCtorDirective = new ConstructorInjectionDirective(_knifeDefaultsCtor, _injectorFactory.Create(_knifeDefaultsCtor));

            _spartanNameAndAgeCtor = typeof(Spartan).GetConstructor(new[] { typeof(string), typeof(int) });
            _spartanNameAndAgeCtorDirective = new ConstructorInjectionDirective(_spartanNameAndAgeCtor, _injectorFactory.Create(_spartanNameAndAgeCtor));

            _spartanHeightAndWeaponCtor = typeof(Spartan).GetConstructor(new[] { typeof(string), typeof(int) });
            _spartanHeightAndWeaponCtorDirective = new ConstructorInjectionDirective(_spartanHeightAndWeaponCtor, _injectorFactory.Create(_spartanHeightAndWeaponCtor));

            _barracksCtor = typeof(Barracks).GetConstructor(new[] { typeof(IWarrior), typeof(IWeapon) });
            _barracksCtorDirective = new ConstructorInjectionDirective(_barracksCtor, _injectorFactory.Create(_barracksCtor));

            _monasteryCtor = typeof(Monastery).GetConstructor(new[] { typeof(IWarrior), typeof(IWeapon) });
            _monasteryCtorDirective = new ConstructorInjectionDirective(_monasteryCtor, _injectorFactory.Create(_monasteryCtor));

            _enumerableCtor = typeof(RequestsEnumerable).GetConstructor(new[] { typeof(IEnumerable<IChild>) });
            _enumerableCtorDirective = new ConstructorInjectionDirective(_enumerableCtor, _injectorFactory.Create(_enumerableCtor));

            _standardConstructorScorer = new StandardConstructorScorer { Settings = ninjectSettings };
        }

        [Benchmark]
        public void Inject()
        {
            _standardConstructorScorer.Score(_contextWithParams, _injectCtorDirective);
        }

        [Benchmark]
        public void DefaultConstructor()
        {
            _standardConstructorScorer.Score(_contextWithParams, _defaultCtorDirective);
        }

        [Benchmark]
        public void OnlyParameters_Match()
        {
            _standardConstructorScorer.Score(_contextWithParams, _knifeCtorDirective);
        }

        [Benchmark]
        public void OnlyParameters_NoMatch()
        {
            _standardConstructorScorer.Score(_contextWithParams, _spartanNameAndAgeCtorDirective);
        }

        [Benchmark]
        public void OnlyBindings_DefaultValues()
        {
            _standardConstructorScorer.Score(_contextWithoutParams, _knifeDefaultsCtorDirective);
        }

        [Benchmark]
        public void OnlyBindings_IEnumerable()
        {
            _standardConstructorScorer.Score(_contextWithoutParams, _enumerableCtorDirective);
        }

        [Benchmark]
        public void OnlyBindings_Match()
        {
            _standardConstructorScorer.Score(_contextWithoutParams, _barracksCtorDirective);
        }

        [Benchmark]
        public void OnlyBindings_NoMatch()
        {
            _standardConstructorScorer.Score(_contextWithoutParams, _monasteryCtorDirective);
        }

        [Benchmark]
        public void ParametersAndBindings()
        {
            _standardConstructorScorer.Score(_contextWithParams, _spartanHeightAndWeaponCtorDirective);
        }

        private static Context CreateContext(IKernel kernel, IReadOnlyList<IParameter> parameters, Type serviceType)
        {
            var request = new Request(typeof(StandardConstructorScorerBenchmark),
                                      null,
                                      parameters,
                                      null,
                                      false,
                                      true);

            return new Context(kernel,
                               request,
                               new Binding(serviceType),
                               kernel.Components.Get<ICache>(),
                               kernel.Components.Get<IPlanner>(),
                               kernel.Components.Get<IPipeline>(),
                               kernel.Components.Get<IExceptionFormatter>());
        }

        public class Monastery
        {
            public Monastery(IWarrior warrior, IWeapon weapon)
            {
                Weapon = weapon;
                Warrior = warrior;
            }

            public IWeapon Weapon { get; }
            public IWarrior Warrior { get; }
        }

        public class Knife
        {
            public Knife(int length, int width)
            {
                Length = length;
                Width = width;
            }

            public Knife(bool sharp, string name = null)
            {
            }

            public int Length { get; }
            public int Width { get; }
        }

        public class Spartan
        {
            public Spartan(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public Spartan(int height, IWeapon weapon)
            {
                Height = height;
                Weapon = weapon;
            }

            public string Name { get; }
            public int Age { get; }
            public int Height { get; }
            public IWeapon Weapon { get; }
        }
    }
}
