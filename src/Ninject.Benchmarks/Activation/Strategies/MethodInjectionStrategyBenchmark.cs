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
    public class MethodInjectionStrategyBenchmark
    {
        private Context _context;
        private InstanceReference _reference;
        private MethodInjectionStrategy _methodInjectionStrategy;

        public MethodInjectionStrategyBenchmark()
        {
            var injectorFactory = new ExpressionInjectorFactory();

            var ninjectSettings = new NinjectSettings { LoadExtensions = false };
            var kernelConfiguration = new KernelConfiguration(ninjectSettings);
            kernelConfiguration.Bind<MyService>().ToSelf();
            kernelConfiguration.Bind<IWarrior>().To<Monk>();
            kernelConfiguration.Bind<IWeapon>().To<Sword>();

            _context = CreateContext(kernelConfiguration,
                                     kernelConfiguration.BuildReadOnlyKernel(),
                                     Array.Empty<IParameter>(),
                                     typeof(MyService),
                                     ninjectSettings);
            _reference = new InstanceReference { Instance = _context.Resolve() };

            _methodInjectionStrategy = new MethodInjectionStrategy();
        }

        [Benchmark]
        public void Activate()
        {
            _methodInjectionStrategy.Activate(_context, _reference);
        }

        private static Context CreateContext(IKernelConfiguration kernelConfiguration, IReadOnlyKernel readonlyKernel, IReadOnlyList<IParameter> parameters, Type serviceType, INinjectSettings ninjectSettings)
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

        public class MyService
        {
            public IWarrior Warrior { get; private set; }
            public IWeapon Weapon { get; private set; }

            public void ConfigureSoldier(IWarrior warrior, IWeapon weapon)
            {
                Warrior = warrior;
                Weapon = weapon;
            }

            public void Run()
            {
            }
        }
    }
}
