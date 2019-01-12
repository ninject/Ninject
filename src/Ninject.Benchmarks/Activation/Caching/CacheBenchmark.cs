using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Strategies;
using Ninject.Components;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ninject.Benchmarks.Activation.Caching
{
    [MemoryDiagnoser]
    public class CacheBenchmark
    {
        private const int PerThreadLoopCount = 5_000;
        private const int ThreadCount = 20;
        private const int TryGetOperationsPerInvoke = 1000;

        private Cache _cache;
        private KernelConfiguration _kernelConfiguration;
        private IReadOnlyKernel _readOnlyKernel;
        private object _instanceForScopeWithZeroEntries;
        private InstanceReference _instanceReferenceForScopeWithZeroEntries;
        private Context _contextWithNoCache;
        private Context _contextWithZeroEntriesForBindingConfiguration;
        private Context _contextWithOneEntryForBindingConfiguration;
        private Context _contextWithMoreThanOneEntryForBindingConfiguration;
        private object _scopeWithNoCache;
        private object _scopeWithOneEntryForBindingConfiguration;
        private object _scopeWithMoreThanOneEntryForBindingConfiguration;
        private object _scopeWithZeroEntriesForBindingConfiguration;

        public CacheBenchmark()
        {
            var ninjectSettings = new NinjectSettings
                {
                    // Disable to reduce memory pressure
                    ActivationCacheDisabled = true,
                    LoadExtensions = false,
                };

            _cache = new Cache(new Pipeline(Enumerable.Empty<IActivationStrategy>(), new ActivationCache(new NoOpCachePruner())), new NoOpCachePruner());

            _kernelConfiguration = new KernelConfiguration();
            _readOnlyKernel = _kernelConfiguration.BuildReadOnlyKernel();
            _instanceForScopeWithZeroEntries = new object();
            _instanceReferenceForScopeWithZeroEntries = new InstanceReference { Instance = _instanceForScopeWithZeroEntries };

            _scopeWithNoCache = new object();
            _scopeWithOneEntryForBindingConfiguration = new object();
            _scopeWithZeroEntriesForBindingConfiguration = new object();
            _scopeWithMoreThanOneEntryForBindingConfiguration = new object();

            _contextWithNoCache = CreateContext(_kernelConfiguration, _readOnlyKernel, typeof(string), _scopeWithNoCache, ninjectSettings);
            _contextWithZeroEntriesForBindingConfiguration = CreateContext(_kernelConfiguration, _readOnlyKernel, typeof(string), _scopeWithZeroEntriesForBindingConfiguration, ninjectSettings);
            _contextWithOneEntryForBindingConfiguration = CreateContext(_kernelConfiguration, _readOnlyKernel, typeof(string), _scopeWithOneEntryForBindingConfiguration, ninjectSettings);
            _contextWithMoreThanOneEntryForBindingConfiguration = CreateContext(_kernelConfiguration, _readOnlyKernel, typeof(string), _scopeWithMoreThanOneEntryForBindingConfiguration, ninjectSettings);
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _cache.Remember(_contextWithOneEntryForBindingConfiguration, _scopeWithOneEntryForBindingConfiguration, new InstanceReference { Instance = new object() });

            _cache.Remember(_contextWithZeroEntriesForBindingConfiguration, _scopeWithZeroEntriesForBindingConfiguration, _instanceReferenceForScopeWithZeroEntries);
            _cache.Release(_instanceForScopeWithZeroEntries);

            _cache.Remember(_contextWithMoreThanOneEntryForBindingConfiguration, _scopeWithMoreThanOneEntryForBindingConfiguration, new InstanceReference { Instance = new object() });
            _cache.Remember(_contextWithMoreThanOneEntryForBindingConfiguration, _scopeWithMoreThanOneEntryForBindingConfiguration, new InstanceReference { Instance = new object() });
            _cache.Remember(_contextWithMoreThanOneEntryForBindingConfiguration, _scopeWithMoreThanOneEntryForBindingConfiguration, new InstanceReference { Instance = new object() });
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _cache.Clear();
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void TryGet_Context_NoCacheForScope()
        {
            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.TryGet(_contextWithNoCache);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void TryGet_Context_OneEntryForBindingConfiguration()
        {
            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.TryGet(_contextWithOneEntryForBindingConfiguration);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void TryGet_Context_ZeroEntriesForBindingConfiguration()
        {
            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.TryGet(_contextWithZeroEntriesForBindingConfiguration);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void TryGet_Context_MoreThanOneEntryForBindingConfiguration()
        {
            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.TryGet(_contextWithMoreThanOneEntryForBindingConfiguration);
            }
        }

        [Benchmark(OperationsPerInvoke= TryGetOperationsPerInvoke)]
        public void TryGet_ContextAndScope_NoCacheForScope()
        {
            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.TryGet(_contextWithNoCache, _scopeWithNoCache);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void TryGet_ContextAndScope_OneEntryForBindingConfiguration()
        {
            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.TryGet(_contextWithOneEntryForBindingConfiguration, _scopeWithOneEntryForBindingConfiguration);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void TryGet_ContextAndScope_ZeroEntriesForBindingConfiguration()
        {
            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.TryGet(_contextWithZeroEntriesForBindingConfiguration, _scopeWithZeroEntriesForBindingConfiguration);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void TryGet_ContextAndScope_MoreThanOneEntryForBindingConfiguration()
        {
            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.TryGet(_contextWithMoreThanOneEntryForBindingConfiguration, _scopeWithMoreThanOneEntryForBindingConfiguration);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void Remember_ContextAndReference_NoCacheForScope()
        {
            var instance = new object();
            var reference = new InstanceReference { Instance = instance };

            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.Remember(_contextWithNoCache, reference);
                _cache.Clear(_scopeWithNoCache);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void Remember_ContextAndScopeAndReference_NoCacheForScope()
        {
            var instance = new object();
            var reference = new InstanceReference { Instance = instance };

            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.Remember(_contextWithNoCache, _scopeWithNoCache, reference);
                _cache.Clear(_scopeWithNoCache);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void Remember_ContextAndReference()
        {
            var instance = new object();
            var reference = new InstanceReference { Instance = instance };

            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.Remember(_contextWithZeroEntriesForBindingConfiguration, reference);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void Remember_ContextAndScopeAndReference()
        {
            var instance = new object();
            var reference = new InstanceReference { Instance = instance };

            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.Remember(_contextWithZeroEntriesForBindingConfiguration, _scopeWithZeroEntriesForBindingConfiguration, reference);
            }
        }

        [Benchmark(OperationsPerInvoke = TryGetOperationsPerInvoke)]
        public void Release_InstanceNotFound_SingleThreaded()
        {
            var instance = new object();

            for (var i = 0; i < TryGetOperationsPerInvoke; i++)
            {
                _cache.Release(instance);
            }
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void Release_InstanceNotFound_MultiThreaded()
        {
            var instance = new object();

            var tasks = Enumerable.Range(1, ThreadCount)
                                  .Select(_ => Task.Factory.StartNew(() =>
                                      {
                                          for (var i = 0; i < PerThreadLoopCount; i++)
                                          {
                                              _cache.Release(instance);
                                          }
                                      },
                                      TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void RememberAndRelease_ContextAndReference_Singlethreaded()
        {
            _cache.Remember(_contextWithMoreThanOneEntryForBindingConfiguration, _instanceReferenceForScopeWithZeroEntries);
            _cache.Release(_instanceForScopeWithZeroEntries);
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void RememberAndRelease_ContextAndReference_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                                  .Select(_ => Task.Factory.StartNew(() =>
                                      {
                                          var instance = new object();
                                          var reference = new InstanceReference { Instance = instance };

                                          for (var i = 0; i < PerThreadLoopCount; i++)
                                          {
                                              _cache.Remember(_contextWithMoreThanOneEntryForBindingConfiguration, reference);
                                              _cache.Release(instance);
                                          }
                                      },
                                      TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void RememberAndRelease_ContextAndScopeAndReference_Singlethreaded()
        {
            _cache.Remember(_contextWithMoreThanOneEntryForBindingConfiguration, _scopeWithOneEntryForBindingConfiguration, _instanceReferenceForScopeWithZeroEntries);
            _cache.Release(_instanceForScopeWithZeroEntries);
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void RememberAndRelease_ContextAndScopeAndReference_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                                  .Select(_ => Task.Factory.StartNew(() =>
                                      {
                                          var instance = new object();
                                          var reference = new InstanceReference { Instance = instance };

                                          for (var i = 0; i < PerThreadLoopCount; i++)
                                          {
                                              _cache.Remember(_contextWithMoreThanOneEntryForBindingConfiguration, _scopeWithOneEntryForBindingConfiguration, reference);
                                              _cache.Release(instance);
                                          }
                                      },
                                      TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        private static Context CreateContext(IKernelConfiguration kernelConfiguration, IReadOnlyKernel readonlyKernel, Type serviceType, object scope, INinjectSettings ninjectSettings)
        {
            var request = new Request(typeof(CacheBenchmark),
                                      null,
                                      Enumerable.Empty<IParameter>(),
                                      null,
                                      false,
                                      true);

            var binding = new Binding(serviceType);
            
            if (scope != null)
            {
                binding.ScopeCallback = ctx => scope;
            }

            return new Context(readonlyKernel,
                               ninjectSettings,
                               request,
                               binding,
                               kernelConfiguration.Components.Get<ICache>(),
                               kernelConfiguration.Components.Get<IPlanner>(),
                               kernelConfiguration.Components.Get<IPipeline>(),
                               kernelConfiguration.Components.Get<IExceptionFormatter>());
        }


        public class NoOpCachePruner : ICachePruner
        {
            public void Dispose()
            {
            }

            public void Start(IPruneable cache)
            {
            }

            public void Stop()
            {
            }
        }
    }
}
