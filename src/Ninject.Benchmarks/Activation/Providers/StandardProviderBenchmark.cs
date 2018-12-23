using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Providers;
using Ninject.Infrastructure.Introspection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Planning.Targets;
using Ninject.Selection.Heuristics;
using Ninject.Tests.Fakes;
using System;
using System.Collections.Generic;

namespace Ninject.Benchmarks.Activation.Providers
{
    [MemoryDiagnoser]
    public class StandardProviderBenchmark
    {
        private Context _contextWithConstructorArguments;
        private ParameterTarget _warriorParameterTarget;
        private StandardProvider _standardProvider;
        private Context _contextWithoutConstructorArguments;
        private Context _contextWithDefaultConstructor;

        public StandardProviderBenchmark()
        {
            var ninjectSettings = new NinjectSettings
                {
                    // Disable to reduce memory pressure
                    ActivationCacheDisabled = true,
                    LoadExtensions = false
                };
            var kernelConfigurationWithoutBindings = new KernelConfiguration(ninjectSettings);

            #region FromConstructorArguments

            _contextWithConstructorArguments = CreateContext(kernelConfigurationWithoutBindings,
                                                             kernelConfigurationWithoutBindings.BuildReadOnlyKernel(),
                                                             new List<IParameter>
                                                                {
                                                                    new ConstructorArgument("location", "Biutiful"),
                                                                    new PropertyValue("warrior", "cutter"),
                                                                    new ConstructorArgument("warrior", new Monk()),
                                                                    new ConstructorArgument("weapon", new Dagger()),
                                                                },
                                                             typeof(NinjaBarracks),
                                                             ninjectSettings);
            _contextWithConstructorArguments.Plan = kernelConfigurationWithoutBindings.Components.Get<IPlanner>().GetPlan(typeof(NinjaBarracks));

            #endregion FromConstructorArguments

            #region FromBindings

            var kernelConfigurationWithBindings = new KernelConfiguration(ninjectSettings);
            kernelConfigurationWithBindings.Bind<IWarrior>().To<Monk>().InSingletonScope();
            kernelConfigurationWithBindings.Bind<IWeapon>().To<Dagger>().InSingletonScope();
            _contextWithoutConstructorArguments = CreateContext(kernelConfigurationWithBindings,
                                                                kernelConfigurationWithBindings.BuildReadOnlyKernel(),
                                                                new List<IParameter>(),
                                                                typeof(NinjaBarracks),
                                                                ninjectSettings);
            _contextWithoutConstructorArguments.Plan = kernelConfigurationWithBindings.Components.Get<IPlanner>().GetPlan(typeof(NinjaBarracks));

            #endregion FromBindings

            #region FromDefaultConstructor

            _contextWithDefaultConstructor = CreateContext(kernelConfigurationWithBindings,
                                                           kernelConfigurationWithBindings.BuildReadOnlyKernel(),
                                                           new List<IParameter>(),
                                                           typeof(Dagger),
                                                           ninjectSettings);
            _contextWithDefaultConstructor.Plan = kernelConfigurationWithBindings.Components.Get<IPlanner>().GetPlan(typeof(Dagger));

            #endregion FromDefaultConstructor

            _warriorParameterTarget = CreateWarriorParameterTarget();

            _standardProvider = new StandardProvider(typeof(StandardProviderBenchmark),
                                                     kernelConfigurationWithoutBindings.Components.Get<IPlanner>(),
                                                     kernelConfigurationWithoutBindings.Components.Get<IConstructorScorer>());

        }

        [Benchmark]
        public void Create_FromConstructorArguments()
        {
            _standardProvider.Create(_contextWithConstructorArguments);
        }

        [Benchmark]
        public void Create_FromBindings()
        {
            _standardProvider.Create(_contextWithoutConstructorArguments);
        }

        [Benchmark]
        public void Create_FromDefaultConstructor()
        {
            _standardProvider.Create(_contextWithDefaultConstructor);
        }

        //[Benchmark]
        public void GetValue_FromConstructorArguments()
        {
            _standardProvider.GetValue(_contextWithConstructorArguments, _warriorParameterTarget);
        }

        private static Context CreateContext(IKernelConfiguration kernelConfiguration, IReadOnlyKernel readonlyKernel, IEnumerable<IParameter> parameters, Type serviceType, INinjectSettings ninjectSettings)
        {
            var request = new Request(typeof(StandardProviderBenchmark),
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

        private static ParameterTarget CreateWarriorParameterTarget()
        {
            var ctor = typeof(NinjaBarracks).GetConstructor(new[] { typeof(IWarrior), typeof(IWeapon) });
            return new ParameterTarget(ctor, ctor.GetParameters()[0]);
        }
    }
}
