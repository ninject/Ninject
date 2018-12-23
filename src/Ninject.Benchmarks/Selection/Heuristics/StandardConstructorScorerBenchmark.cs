using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Infrastructure.Introspection;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Planning.Directives;
using Ninject.Selection.Heuristics;
using Ninject.Tests.Fakes;
using Ninject.Tests.Integration;

namespace Ninject.Benchmarks.Selection.Heuristics
{
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
        private StandardConstructorScorer _standardConstructorScorer;
        private IInjectorFactory _injectorFactory;
        private Context _contextWithParams;
        private Context _contextWithoutParams;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var ninjectSettings = new NinjectSettings { LoadExtensions = false };

            var kernelConfiguration = new KernelConfiguration(ninjectSettings);
            kernelConfiguration.Bind<IWarrior>().To<SpecialNinja>().WhenInjectedExactlyInto<NinjaBarracks>();
            kernelConfiguration.Bind<IWarrior>().To<Samurai>().WhenInjectedExactlyInto<Barracks>();
            kernelConfiguration.Bind<IWarrior>().To<FootSoldier>().WhenInjectedExactlyInto<Barracks>();
            kernelConfiguration.Bind<IWeapon>().To<Shuriken>().WhenInjectedExactlyInto<Barracks>();
            kernelConfiguration.Bind<IWeapon>().To<ShortSword>().WhenInjectedExactlyInto<Spartan>();

            _injectorFactory = new ExpressionInjectorFactory();

            _contextWithParams = CreateContext(kernelConfiguration,
                                               kernelConfiguration.BuildReadOnlyKernel(),
                                               new List<IParameter>
                                                    {
                                                        new ConstructorArgument("height", 34),
                                                        new PropertyValue("name", "cutter"),
                                                        new ConstructorArgument("width", 17),
                                                        new ConstructorArgument("location", "Biutiful")
                                                    },
                                               typeof(StandardConstructorScorerBenchmark),
                                               ninjectSettings);

            _contextWithoutParams = CreateContext(kernelConfiguration,
                                                  kernelConfiguration.BuildReadOnlyKernel(),
                                                  Enumerable.Empty<IParameter>(),
                                                  typeof(StandardConstructorScorerBenchmark),
                                                  ninjectSettings);

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

            _standardConstructorScorer = new StandardConstructorScorer(ninjectSettings);
        }

        //[Benchmark]
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

        //[Benchmark]
        public void OnlyBindings_DefaultValues()
        {
            _standardConstructorScorer.Score(_contextWithoutParams, _knifeDefaultsCtorDirective);
        }

        //[Benchmark]
        public void OnlyBindings_Match()
        {
            _standardConstructorScorer.Score(_contextWithoutParams, _barracksCtorDirective);
        }

        //[Benchmark]
        public void OnlyBindings_NoMatch()
        {
            _standardConstructorScorer.Score(_contextWithoutParams, _monasteryCtorDirective);
        }

        //[Benchmark]
        public void ParametersAndBindings()
        {
            _standardConstructorScorer.Score(_contextWithParams, _spartanHeightAndWeaponCtorDirective);
        }

        private static Context CreateContext(IKernelConfiguration kernelConfiguration, IReadOnlyKernel readonlyKernel, IEnumerable<IParameter> parameters, Type serviceType, INinjectSettings ninjectSettings)
        {
            var request = new Request(typeof(StandardConstructorScorerBenchmark),
                                      null,
                                      parameters,
                                      null,
                                      false,
                                      true);

            return new Context(readonlyKernel,
                               ninjectSettings,
                               request,
                               new Binding(serviceType),
                               kernelConfiguration.Components.Get<ICache>(),
                               kernelConfiguration.Components.Get<IPlanner>(),
                               kernelConfiguration.Components.Get<IPipeline>(),
                               kernelConfiguration.Components.Get<IExceptionFormatter>());
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
