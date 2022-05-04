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
            var injectorFactory = new DynamicMethodInjectorFactory();

            var ninjectSettings = new NinjectSettings { LoadExtensions = false };
            var kernel = new StandardKernel(ninjectSettings);
            kernel.Bind<MyService>().ToSelf();
            kernel.Bind<IWarrior>().To<Monk>();
            kernel.Bind<IWeapon>().To<Sword>();

            _context = CreateContext(kernel,
                                     Array.Empty<IParameter>(),
                                     typeof(MyService));
            _reference = new InstanceReference { Instance = _context.Resolve() };

            _methodInjectionStrategy = new MethodInjectionStrategy();
        }

        [Benchmark]
        public void Activate()
        {
            _methodInjectionStrategy.Activate(_context, _reference);
        }

        private static Context CreateContext(IKernel kernel, IReadOnlyList<IParameter> parameters, Type serviceType)
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
