using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Strategies;
using Ninject.Components;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Tests.Fakes;

namespace Ninject.Benchmarks.Activation.Strategies
{
    [MemoryDiagnoser]
    public class PropertyInjectionStrategyBenchmark
    {
        private InstanceReference _bareReferenceWithPropertyValuesFullMatch;
        private InstanceReference _bareReferenceWithPropertyValuesPartialMatch;
        private InstanceReference _bareReferenceWithoutPropertyValues;
        private InstanceReference _instrumentedReferenceWithPropertyValuesFullMatch;
        private InstanceReference _instrumentedReferenceWithPropertyValuesPartialMatch;
        private InstanceReference _instrumentedReferenceWithoutPropertyValues;
        private PropertyInjectionStrategy _propertyInjectionStrategy;
        private Context _instrumentedContextWithPropertyValuesFullMatch;
        private Context _instrumentedContextWithPropertyValuesPartialMatch;
        private Context _instrumentedContextWithoutPropertyValues;
        private Context _bareContextWithPropertyValuesFullMatch;
        private Context _bareContextWithPropertyValuesPartialMatch;
        private Context _bareContextWithoutPropertyValues;

        public PropertyInjectionStrategyBenchmark()
        {
            var ninjectSettings = new NinjectSettings
                {
                    // Disable to reduce memory pressure
                    ActivationCacheDisabled = true,
                    LoadExtensions = false
                };
            var kernel = new StandardKernel(ninjectSettings);
            kernel.Bind<MyInstrumentedService>().ToSelf();
            kernel.Bind<MyBareService>().ToSelf();
            kernel.Bind<IWarrior>().To<Monk>();
            kernel.Bind<IWeapon>().To<Sword>();
            kernel.Bind<ICleric>().To<Monk>();

            var propertyValuesFullMatch = new List<IPropertyValue>
                {
                    new PropertyValue(nameof(MyInstrumentedService.Warrior), new FootSoldier()),
                    new PropertyValue(nameof(MyInstrumentedService.Weapon), new Dagger()),
                    new PropertyValue(nameof(MyInstrumentedService.Cleric), new Monk())
                };
            var propertyValuesPartialMatch = new List<IPropertyValue>
                {
                    new PropertyValue(nameof(MyInstrumentedService.Weapon), new Dagger()),
                    new PropertyValue(nameof(MyInstrumentedService.Cleric), new Monk())
                };

            _instrumentedContextWithPropertyValuesFullMatch = CreateContext(kernel,
                                                                            propertyValuesFullMatch,
                                                                            typeof(MyInstrumentedService),
                                                                            ninjectSettings);
            _instrumentedContextWithPropertyValuesPartialMatch = CreateContext(kernel,
                                                                               propertyValuesPartialMatch,
                                                                               typeof(MyInstrumentedService),
                                                                               ninjectSettings);
            _instrumentedContextWithoutPropertyValues = CreateContext(kernel,
                                                                      Array.Empty<IParameter>(),
                                                                      typeof(MyInstrumentedService),
                                                                      ninjectSettings);
            _bareContextWithPropertyValuesFullMatch = CreateContext(kernel,
                                                                    propertyValuesFullMatch,
                                                                    typeof(MyBareService),
                                                                    ninjectSettings);
            _bareContextWithPropertyValuesPartialMatch = CreateContext(kernel,
                                                                       propertyValuesPartialMatch,
                                                                       typeof(MyBareService),
                                                                       ninjectSettings);
            _bareContextWithoutPropertyValues = CreateContext(kernel,
                                                              Array.Empty<IParameter>(),
                                                              typeof(MyBareService),
                                                              ninjectSettings);

            _bareReferenceWithPropertyValuesFullMatch = new InstanceReference { Instance = _bareContextWithPropertyValuesFullMatch.Resolve() };
            _bareReferenceWithPropertyValuesPartialMatch = new InstanceReference { Instance = _bareContextWithPropertyValuesPartialMatch.Resolve() };
            _bareReferenceWithoutPropertyValues = new InstanceReference { Instance = _bareContextWithoutPropertyValues.Resolve() };
            _instrumentedReferenceWithPropertyValuesFullMatch = new InstanceReference { Instance = _instrumentedContextWithPropertyValuesFullMatch.Resolve() };
            _instrumentedReferenceWithPropertyValuesPartialMatch = new InstanceReference { Instance = _instrumentedContextWithPropertyValuesPartialMatch.Resolve() };
            _instrumentedReferenceWithoutPropertyValues = new InstanceReference { Instance = _instrumentedContextWithoutPropertyValues.Resolve() };

            _propertyInjectionStrategy = new PropertyInjectionStrategy(new DynamicMethodInjectorFactory(), new ExceptionFormatter()) { Settings = ninjectSettings };
        }

        [Benchmark]
        public void Activate_WithoutPropertyInjectionDirectivesAndWithPropertyValuesFullMatch()
        {
            _propertyInjectionStrategy.Activate(_bareContextWithPropertyValuesFullMatch, _bareReferenceWithPropertyValuesFullMatch);
        }

        [Benchmark]
        public void Activate_WithoutPropertyInjectionDirectivesAndWithPropertyValuesPartialMatch()
        {
            _propertyInjectionStrategy.Activate(_bareContextWithPropertyValuesPartialMatch, _bareReferenceWithPropertyValuesPartialMatch);
        }

        [Benchmark]
        public void Activate_WithoutPropertyInjectionDirectivesAndWithoutPropertyValues()
        {
            _propertyInjectionStrategy.Activate(_bareContextWithoutPropertyValues, _bareReferenceWithoutPropertyValues);
        }

        [Benchmark]
        public void Activate_WithPropertyInjectionDirectivesAndWithPropertyValuesFullMatch()
        {
            _propertyInjectionStrategy.Activate(_instrumentedContextWithPropertyValuesFullMatch, _instrumentedReferenceWithPropertyValuesFullMatch);
        }

        [Benchmark]
        public void Activate_WithPropertyInjectionDirectivesAndWithPropertyValuesPartialMatch()
        {
            _propertyInjectionStrategy.Activate(_instrumentedContextWithPropertyValuesPartialMatch, _instrumentedReferenceWithPropertyValuesPartialMatch);
        }

        [Benchmark]
        public void Activate_WithPropertyInjectionDirectivesAndWithoutPropertyValues()
        {
            _propertyInjectionStrategy.Activate(_instrumentedContextWithoutPropertyValues, _instrumentedReferenceWithoutPropertyValues);
        }

        private static Context CreateContext(IKernel kernel, IReadOnlyList<IParameter> parameters, Type serviceType, INinjectSettings ninjectSettings)
        {
            var request = new Request(serviceType,
                                      null,
                                      parameters,
                                      null,
                                      false,
                                      true);

            var binding = kernel.GetBindings(serviceType).Single();

            return new Context(kernel,
                               request,
                               binding,
                               kernel.Components.Get<ICache>(),
                               kernel.Components.Get<IPlanner>(),
                               kernel.Components.Get<IPipeline>(),
                               kernel.Components.Get<IExceptionFormatter>());
        }

        public class MyInstrumentedService
        {
            [Inject]
            public IWarrior Warrior { get; set; }

            [Inject]
            public IWeapon Weapon { get; set; }

            [Inject]
            public ICleric Cleric { get; set; }
        }

        public class MyBareService
        {
            public IWarrior Warrior { get; set; }

            public IWeapon Weapon { get; set; }

            public ICleric Cleric { get; set; }
        }
    }
}
