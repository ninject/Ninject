using BenchmarkDotNet.Attributes;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Strategies;
using Ninject.Components;
using System.Collections.Generic;

namespace Ninject.Benchmarks.Activation
{
    [MemoryDiagnoser]
    public class PipelineBenchmark
    {
        private Pipeline _pipelineWithoutStrategies;
        private Pipeline _pipelineWithStrategies;
        private InstanceReference _activatedReference;
        private InstanceReference _deactivatedReference;
        private IContext _context;

        public PipelineBenchmark()
        {
            var cachePruner = new GarbageCollectionCachePruner();
            var activationCache = new ActivationCache(cachePruner);

            for (var i = 0; i < 1000; i++)
            {
                activationCache.AddActivatedInstance("" + i);
                activationCache.AddDeactivatedInstance("" + i);
            }

            var noStrategies = new List<IActivationStrategy>();
            _pipelineWithoutStrategies = new Pipeline(noStrategies, activationCache);

            var strategies = new List<IActivationStrategy> {
                                                            new NoOpStrategy(),
                                                            new NoOpStrategy(),
                                                            new NoOpStrategy(),
                                                            new NoOpStrategy(),
                                                            new NoOpStrategy(),
                                                            new NoOpStrategy()
                                                           };
            _pipelineWithStrategies = new Pipeline(strategies, activationCache);

            _activatedReference = new InstanceReference { Instance = "ACTIVATED" };
            activationCache.AddActivatedInstance(_activatedReference.Instance);

            _deactivatedReference = new InstanceReference { Instance = "DEACTIVATED" };
            activationCache.AddDeactivatedInstance(_deactivatedReference.Instance);

            _context = new Mock<IContext>(MockBehavior.Strict).Object;
        }

        [Benchmark]
        public void Activate_WithStrategies_ObjectActivated()
        {
            _pipelineWithStrategies.Activate(_context, _activatedReference);
        }

        [Benchmark]
        public void Activate_WithStrategies_ObjectNotActivated()
        {
            _pipelineWithStrategies.Activate(_context, _deactivatedReference);
        }

        [Benchmark]
        public void Activate_WithoutStrategies_ObjectActivated()
        {
            _pipelineWithoutStrategies.Activate(_context, _activatedReference);
        }

        [Benchmark]
        public void Activate_WithoutStrategies_ObjectNotActivated()
        {
            _pipelineWithoutStrategies.Activate(_context, _deactivatedReference);
        }

        [Benchmark]
        public void Deactivate_WithStrategies_ObjectDeactivated()
        {
            _pipelineWithStrategies.Deactivate(_context, _deactivatedReference);
        }

        [Benchmark]
        public void Deactivate_WithStrategies_ObjectNotDeactivated()
        {
            _pipelineWithStrategies.Deactivate(_context, _activatedReference);
        }

        [Benchmark]
        public void Deactivate_WithoutStrategies_ObjectDeactivated()
        {
            _pipelineWithoutStrategies.Deactivate(_context, _deactivatedReference);
        }

        [Benchmark]
        public void Deactivate_WithoutStrategies_ObjectNotDeactivated()
        {
            _pipelineWithoutStrategies.Deactivate(_context, _activatedReference);
        }

        public class NoOpStrategy : NinjectComponent, IActivationStrategy
        {
            public void Activate(IContext context, InstanceReference reference)
            {
            }

            public void Deactivate(IContext context, InstanceReference reference)
            {
            }
        }
    }
}
