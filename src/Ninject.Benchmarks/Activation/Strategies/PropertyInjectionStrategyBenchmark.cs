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
            var kernelConfiguration = new KernelConfiguration(ninjectSettings);
            kernelConfiguration.Bind<MyInstrumentedService>().ToSelf();
            kernelConfiguration.Bind<MyBareService>().ToSelf();
            kernelConfiguration.Bind<IWarrior>().To<Monk>();
            kernelConfiguration.Bind<IWeapon>().To<Sword>();
            kernelConfiguration.Bind<ICleric>().To<Monk>();

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

            _instrumentedContextWithPropertyValuesFullMatch = CreateContext(kernelConfiguration,
                                                                            kernelConfiguration.BuildReadOnlyKernel(),
                                                                            propertyValuesFullMatch,
                                                                            typeof(MyInstrumentedService),
                                                                            ninjectSettings);
            _instrumentedContextWithPropertyValuesPartialMatch = CreateContext(kernelConfiguration,
                                                                               kernelConfiguration.BuildReadOnlyKernel(),
                                                                               propertyValuesPartialMatch,
                                                                               typeof(MyInstrumentedService),
                                                                               ninjectSettings);
            _instrumentedContextWithoutPropertyValues = CreateContext(kernelConfiguration,
                                                                      kernelConfiguration.BuildReadOnlyKernel(),
                                                                      Enumerable.Empty<IParameter>(),
                                                                      typeof(MyInstrumentedService),
                                                                      ninjectSettings);
            _bareContextWithPropertyValuesFullMatch = CreateContext(kernelConfiguration,
                                                                    kernelConfiguration.BuildReadOnlyKernel(),
                                                                    propertyValuesFullMatch,
                                                                    typeof(MyBareService),
                                                                    ninjectSettings);
            _bareContextWithPropertyValuesPartialMatch = CreateContext(kernelConfiguration,
                                                                       kernelConfiguration.BuildReadOnlyKernel(),
                                                                       propertyValuesPartialMatch,
                                                                       typeof(MyBareService),
                                                                       ninjectSettings);
            _bareContextWithoutPropertyValues = CreateContext(kernelConfiguration,
                                                              kernelConfiguration.BuildReadOnlyKernel(),
                                                              Enumerable.Empty<IParameter>(),
                                                              typeof(MyBareService),
                                                              ninjectSettings);

            _bareReferenceWithPropertyValuesFullMatch = new InstanceReference { Instance = _bareContextWithPropertyValuesFullMatch.Resolve() };
            _bareReferenceWithPropertyValuesPartialMatch = new InstanceReference { Instance = _bareContextWithPropertyValuesPartialMatch.Resolve() };
            _bareReferenceWithoutPropertyValues = new InstanceReference { Instance = _bareContextWithoutPropertyValues.Resolve() };
            _instrumentedReferenceWithPropertyValuesFullMatch = new InstanceReference { Instance = _instrumentedContextWithPropertyValuesFullMatch.Resolve() };
            _instrumentedReferenceWithPropertyValuesPartialMatch = new InstanceReference { Instance = _instrumentedContextWithPropertyValuesPartialMatch.Resolve() };
            _instrumentedReferenceWithoutPropertyValues = new InstanceReference { Instance = _instrumentedContextWithoutPropertyValues.Resolve() };

            _propertyInjectionStrategy = new PropertyInjectionStrategy(new ExpressionInjectorFactory(), ninjectSettings, new ExceptionFormatter());
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

        private static Context CreateContext(IKernelConfiguration kernelConfiguration, IReadOnlyKernel readonlyKernel, IEnumerable<IParameter> parameters, Type serviceType, INinjectSettings ninjectSettings)
        {
            var request = new Request(serviceType,
                                      null,
                                      parameters,
                                      null,
                                      false,
                                      true);

            var binding = kernelConfiguration.GetBindings(serviceType).Single();

            return new Context(readonlyKernel,
                               ninjectSettings,
                               request,
                               binding,
                               kernelConfiguration.Components.Get<ICache>(),
                               kernelConfiguration.Components.Get<IPlanner>(),
                               kernelConfiguration.Components.Get<IPipeline>(),
                               kernelConfiguration.Components.Get<IExceptionFormatter>());
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
