using BenchmarkDotNet.Attributes;
using Moq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Strategies;
using Ninject.Components;
using Ninject.Injection;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ninject.Benchmarks.Components
{
    [MemoryDiagnoser]
    public class ComponentContainerBenchmark
    {
        private ComponentContainer _container;
        private Dictionary<Type, INinjectComponent> _instanceCache;

        public ComponentContainerBenchmark()
        {
            _container = new ComponentContainer();
            _container.Kernel = new Mock<IKernel>(MockBehavior.Strict).Object;
            _container.Add<IInjectorFactory, DynamicMethodInjectorFactory>();
            _container.Add<IActivationStrategy, PropertyInjectionStrategy>();
            _container.Add<IActivationStrategy, MethodInjectionStrategy>();
            _container.Add<IPipeline, Pipeline>();
            _container.Add<ICachePruner, GarbageCollectionCachePruner>();
            _container.Add<IActivationCache, ActivationCache>();

            _instanceCache = GetInstanceCache(_container);
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _container.Get(typeof(IPipeline));
        }

        [Benchmark]
        public void Get_INinjectSettings()
        {
            var settings = _container.Get(typeof(INinjectSettings));
            if (settings == null)
                throw new Exception();
        }

        [Benchmark]
        public void Get_IKernel()
        {
            var kernel = _container.Get(typeof(IKernel));
            if (kernel == null)
                throw new Exception();
        }

        [Benchmark]
        public void Get_ComponentRegistered_Cached()
        {
            var pipeline = _container.Get(typeof(IPipeline));
            if (pipeline == null)
                throw new Exception();
        }

        [Benchmark]
        public void Get_ComponentRegistered_NotCached()
        {
            _instanceCache.Remove(typeof(Pipeline));

            var pipeline = _container.Get(typeof(IPipeline));
            if (pipeline == null)
                throw new Exception();
        }

        [Benchmark]
        public void Get_NoSuchComponentRegistered()
        {
            try
            {
                _container.Get(typeof(IModuleLoader));
                throw new Exception();
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Benchmark]
        public void Get_IEnumerable()
        {
            var entries = _container.Get(typeof(IEnumerable<IActivationStrategy>));
            if (entries == null)
                throw new Exception();
        }

        [Benchmark]
        public void GetOfT_ComponentRegistered_Cached()
        {
            var pipeline = _container.Get<IPipeline>();
            if (pipeline == null)
                throw new Exception();
        }

        [Benchmark]
        public void GetOfT_ComponentRegistered_NotCached()
        {
            _instanceCache.Remove(typeof(Pipeline));

            var pipeline = _container.Get<IPipeline>();
            if (pipeline == null)
                throw new Exception();
        }

        [Benchmark]
        public void GetOfT_NoSuchComponentRegistered()
        {
            try
            {
                _container.Get<IModuleLoader>();
                throw new Exception();
            }
            catch (InvalidOperationException)
            {
            }
        }

        private static Dictionary<Type, INinjectComponent> GetInstanceCache(ComponentContainer container)
        {
            var instancesField = container.GetType().GetField("instances", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Dictionary<Type, INinjectComponent>) instancesField.GetValue(container);
        }
    }
}
