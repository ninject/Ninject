using BenchmarkDotNet.Attributes;
using Ninject.Planning.Bindings;
using Ninject.Tests.Fakes;
using Ninject.Tests.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Ninject.Benchmarks
{
    [MemoryDiagnoser]
    public class ReadOnlyKernelBenchmark
    {
        private const int MultithreadedLoopCount = 10_000;

        private KernelConfiguration _kernelConfiguration;
        private IReadOnlyKernel _readOnlyKernel;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var ninjectSettings = new NinjectSettings
                {
                    // Disable to reduce memory pressure
                    ActivationCacheDisabled = true,
                    LoadExtensions = false
                };

            _kernelConfiguration = new KernelConfiguration(ninjectSettings);
            _kernelConfiguration.Bind<IWarrior>().To<SpecialNinja>().WhenInjectedExactlyInto<NinjaBarracks>();
            _kernelConfiguration.Bind<IWarrior>().To<Samurai>().WhenInjectedExactlyInto<Barracks>();
            _kernelConfiguration.Bind<IWarrior>().To<FootSoldier>().WhenInjectedExactlyInto<Barracks>();
            _kernelConfiguration.Bind<IWarrior>().To<FootSoldier>();
            _kernelConfiguration.Bind<IWeapon>().To<Shuriken>().WhenInjectedExactlyInto<Barracks>();
            _kernelConfiguration.Bind<IWeapon>().To<Sword>();
            _kernelConfiguration.Bind<ICleric>().To<Monk>();
            _kernelConfiguration.Bind(typeof(ICollection<>)).To(typeof(List<>));
            _kernelConfiguration.Bind(typeof(IList<>)).To(typeof(List<>));
            _kernelConfiguration.Bind<ISingleton>().To<Singleton>().InSingletonScope();
            _kernelConfiguration.Bind<IRouteProvider>().To<RouteProvider>();
            _kernelConfiguration.Bind<IRouteRepository>().To<RouteRepository>();
            _kernelConfiguration.Bind<ITrainPlanner>().To<TrainPlanner>();
            _kernelConfiguration.Bind<IRealtimeEventSource>().To<RealtimeEventSource>();

            _readOnlyKernel = _kernelConfiguration.BuildReadOnlyKernel();
            ClearCache(_readOnlyKernel);
        }

        [Benchmark]
        public void GetBindings_SingleResult()
        {
            var bindings = _readOnlyKernel.GetBindings(typeof(ICleric));
            foreach (var binding in bindings)
            {
                if (binding.Service == null)
                    throw new Exception();
            }
            ClearCache(_readOnlyKernel);
        }

        [Benchmark]
        public void GetBindings_MultipleResults()
        {
            var bindings = _readOnlyKernel.GetBindings(typeof(IWarrior));
            foreach (var binding in bindings)
            {
                if (binding == null)
                    throw new Exception();
            }
            ClearCache(_readOnlyKernel);
        }

        [Benchmark]
        public void GetBindings_EmptyResult()
        {
            var bindings = _readOnlyKernel.GetBindings(typeof(IReflect));
            foreach (var binding in bindings)
            {
                if (binding == null)
                    throw new Exception();
            }
            ClearCache(_readOnlyKernel);
        }

        [Benchmark]
        public void GetBindings_Mixed()
        {
            _readOnlyKernel.GetBindings(typeof(IWarrior));
            _readOnlyKernel.GetBindings(typeof(IReflect));
            _readOnlyKernel.GetBindings(typeof(IWeapon));
            _readOnlyKernel.GetBindings(typeof(ICleric));
            ClearCache(_readOnlyKernel);
        }

        [Benchmark]
        public void GetOfT_Transient_Singlethreaded()
        {
            _readOnlyKernel.Get<ICleric>();
        }

        [Benchmark(OperationsPerInvoke = MultithreadedLoopCount)]
        public void GetOfT_Transient_Multithreaded()
        {
            const int numberOfThreads = 20;
            int counter = 0;

            var tasks = Enumerable.Range(0, numberOfThreads)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        var incremented = Interlocked.Increment(ref counter);
                        if (incremented > MultithreadedLoopCount)
                        {
                            break;
                        }

                        _readOnlyKernel.Get<ICleric>();
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void GetOfT_Singleton_Singlethreaded()
        {
            _readOnlyKernel.Get<ISingleton>();
        }

        [Benchmark(OperationsPerInvoke = MultithreadedLoopCount)]
        public void GetOfT_Singleton_Multithreaded()
        {
            const int numberOfThreads = 20;
            int counter = 0;

            var tasks = Enumerable.Range(0, numberOfThreads)
                .Select(_ => Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            var incremented = Interlocked.Increment(ref counter);
                            if (incremented > MultithreadedLoopCount)
                            {
                                break;
                            }

                            _readOnlyKernel.Get<ISingleton>();
                        }
                    }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void GetOfT_Complex_Singlethreaded()
        {
            _readOnlyKernel.Get<TrainService>();
        }

        [Benchmark(OperationsPerInvoke = MultithreadedLoopCount)]
        public void GetOfT_Complex_Multithreaded()
        {
            const int numberOfThreads = 20;
            int counter = 0;

            var tasks = Enumerable.Range(0, numberOfThreads)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        var incremented = Interlocked.Increment(ref counter);
                        if (incremented > MultithreadedLoopCount)
                        {
                            break;
                        }

                        _readOnlyKernel.Get<TrainService>();
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        private static void ClearCache(IReadOnlyKernel readOnlyKernel)
        {
            var bindingCacheField = typeof(ReadOnlyKernel).GetField("bindingCache", BindingFlags.NonPublic | BindingFlags.Instance);
            var cache = (Dictionary<Type, IBinding[]>)bindingCacheField.GetValue(readOnlyKernel);
            // var cache = (Dictionary<Type, ICollection<IBinding>>)bindingCacheField.GetValue(readOnlyKernel);
            cache.Clear();
        }

        public interface ISingleton
        {
        }

        public class Singleton : ISingleton
        {
        }

        public class TrainService
        {
            public TrainService(ISingleton singleton, ICleric cleric, IRouteProvider routeProvider, ITrainPlanner trainPlanner)
            {
            }
        }

        public class RouteProvider : IRouteProvider
        {
            public RouteProvider(IRouteRepository routeRepository, IRealtimeEventSource realtimeEventSource)
            {
            }
        }

        public class RouteRepository : IRouteRepository
        {
            public RouteRepository(ISingleton singleton, ICleric cleric, IRealtimeEventSource realtimeEventSource)
            {
            }
        }

        public class TrainPlanner : ITrainPlanner
        {
            public TrainPlanner(ISingleton singleton, IRouteRepository routeRepository, IRealtimeEventSource realtimeEventSource)
            {
            }
        }

        public class RealtimeEventSource : IRealtimeEventSource
        {
        }

        public interface IRouteProvider
        {
        }

        public interface IRouteRepository
        {
        }

        public interface ITrainPlanner
        {
        }

        public interface IRealtimeEventSource
        {
        }
    }
}
