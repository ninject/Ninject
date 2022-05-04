using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Providers;
using Ninject.Components;
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
            var kernelWithoutBindings = new StandardKernel(ninjectSettings);

            #region FromConstructorArguments

            _contextWithConstructorArguments = CreateContext(kernelWithoutBindings,
                                                             new List<IParameter>
                                                                {
                                                                    new ConstructorArgument("location", "Biutiful"),
                                                                    new PropertyValue("warrior", "cutter"),
                                                                    new ConstructorArgument("warrior", new Monk()),
                                                                    new ConstructorArgument("weapon", new Dagger()),
                                                                },
                                                             typeof(NinjaBarracks));
            _contextWithConstructorArguments.Plan = kernelWithoutBindings.Components.Get<IPlanner>().GetPlan(typeof(NinjaBarracks));

            #endregion FromConstructorArguments

            #region FromBindings

            var kernelnWithBindings = new StandardKernel(ninjectSettings);
            kernelnWithBindings.Bind<IWarrior>().To<Monk>().InSingletonScope();
            kernelnWithBindings.Bind<IWeapon>().To<Dagger>().InSingletonScope();
            _contextWithoutConstructorArguments = CreateContext(kernelnWithBindings,
                                                                new List<IParameter>(),
                                                                typeof(NinjaBarracks));
            _contextWithoutConstructorArguments.Plan = kernelnWithBindings.Components.Get<IPlanner>().GetPlan(typeof(NinjaBarracks));

            #endregion FromBindings

            #region FromDefaultConstructor

            _contextWithDefaultConstructor = CreateContext(kernelnWithBindings,
                                                           new List<IParameter>(),
                                                           typeof(Dagger));
            _contextWithDefaultConstructor.Plan = kernelnWithBindings.Components.Get<IPlanner>().GetPlan(typeof(Dagger));

            #endregion FromDefaultConstructor

            _warriorParameterTarget = CreateWarriorParameterTarget();

            _standardProvider = new StandardProvider(typeof(StandardProviderBenchmark),
                                                     kernelWithoutBindings.Components.Get<IPlanner>(),
                                                     kernelWithoutBindings.Components.Get<IConstructorScorer>());

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

        private static Context CreateContext(IKernel kernel, IReadOnlyList<IParameter> parameters, Type serviceType)
        {
            var request = new Request(typeof(StandardProviderBenchmark),
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

        private static ParameterTarget CreateWarriorParameterTarget()
        {
            var ctor = typeof(NinjaBarracks).GetConstructor(new[] { typeof(IWarrior), typeof(IWeapon) });
            return new ParameterTarget(ctor, ctor.GetParameters()[0]);
        }
    }
}
