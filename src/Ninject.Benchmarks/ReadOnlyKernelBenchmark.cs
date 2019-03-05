using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Tests.Fakes;
using Ninject.Tests.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Ninject.Benchmarks
{
    [MemoryDiagnoser]
    public class ReadOnlyKernelBenchmark
    {
        private const int PerThreadLoopCount = 1_000;
        private const int ThreadCount = 20;

        private IReadOnlyKernel _kernelWithConstructorAndPropertyInjection;
        private IReadOnlyKernel _kernelWithOnlyConstructorInjection;
        private IRequest _weaponRequest;
        private IRequest _clericRequest;
        private IRequest _reflectRequest;
        private IRequest _barracksRequest;
        private IRequest _leasureRequest;
        private KernelConfiguration _kernelConfigurationWithConstructorAndPropertyInjection;
        private KernelConfiguration _kernelConfigurationWithOnlyConstructorInjection;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var ninjectSettingsWithConstructorAndPropertyInjection = new NinjectSettings
                {
                    // Disable to reduce memory pressure
                    ActivationCacheDisabled = true,
                    LoadExtensions = false,
                    PropertyInjection = true,
                    MethodInjection = false
                };
            var ninjectSettingsWithOnlyConstructorInjection = new NinjectSettings
                {
                    // Disable to reduce memory pressure
                    ActivationCacheDisabled = true,
                    LoadExtensions = false,
                    PropertyInjection = false,
                    MethodInjection = false
                };

            _kernelConfigurationWithConstructorAndPropertyInjection = CreateKernelConfiguration(ninjectSettingsWithConstructorAndPropertyInjection);
            _kernelConfigurationWithOnlyConstructorInjection = CreateKernelConfiguration(ninjectSettingsWithOnlyConstructorInjection);

            _kernelWithConstructorAndPropertyInjection = _kernelConfigurationWithConstructorAndPropertyInjection.BuildReadOnlyKernel();
            _kernelWithOnlyConstructorInjection = _kernelConfigurationWithOnlyConstructorInjection.BuildReadOnlyKernel();

            _weaponRequest = _kernelWithConstructorAndPropertyInjection.CreateRequest(typeof(IWeapon), null, Array.Empty<IParameter>(), false, true);
            _clericRequest = _kernelWithConstructorAndPropertyInjection.CreateRequest(typeof(ICleric), null, Array.Empty<IParameter>(), false, true);
            _reflectRequest = _kernelWithConstructorAndPropertyInjection.CreateRequest(typeof(IReflect), null, Array.Empty<IParameter>(), false, true);
            _barracksRequest = _kernelWithConstructorAndPropertyInjection.CreateRequest(typeof(NinjaBarracks), null, Array.Empty<IParameter>(), false, true);
            _leasureRequest = _kernelWithConstructorAndPropertyInjection.CreateRequest(typeof(ILeasure), null, new IParameter[] { new ConstructorArgument("immediately", true, true) }, false, true);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _kernelConfigurationWithConstructorAndPropertyInjection.Components.Get<ICache>().Clear();
            _kernelConfigurationWithOnlyConstructorInjection.Components.Get<ICache>().Clear();
        }

        [Benchmark]
        public void CanResolve_Request_MultipleBindings()
        {
            if (!_kernelWithConstructorAndPropertyInjection.CanResolve(_weaponRequest))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void CanResolve_Request_SingleBinding()
        {
            if (!_kernelWithConstructorAndPropertyInjection.CanResolve(_clericRequest))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void CanResolve_Request_NoBindings()
        {
            if (_kernelWithConstructorAndPropertyInjection.CanResolve(_reflectRequest))
            {
                throw new Exception();
            }
        }

        [Benchmark]
        public void GetBindings_FromCache()
        {
            var bindings = _kernelWithConstructorAndPropertyInjection.GetBindings(typeof(ICleric));
            foreach (var binding in bindings)
            {
                if (binding.Service == null)
                    throw new Exception();
            }
        }

        [Benchmark]
        public void GetBindings_SingleResult()
        {
            var bindings = _kernelWithConstructorAndPropertyInjection.GetBindings(typeof(ICleric));
            foreach (var binding in bindings)
            {
                if (binding.Service == null)
                    throw new Exception();
            }
            GetBindingCache(_kernelWithConstructorAndPropertyInjection).Clear();
        }

        [Benchmark]
        public void GetBindings_MultipleResults()
        {
            var bindings = _kernelWithConstructorAndPropertyInjection.GetBindings(typeof(IWarrior));
            foreach (var binding in bindings)
            {
                if (binding == null)
                    throw new Exception();
            }
            GetBindingCache(_kernelWithConstructorAndPropertyInjection).Clear();
        }

        [Benchmark]
        public void GetBindings_EmptyResult()
        {
            var bindings = _kernelWithConstructorAndPropertyInjection.GetBindings(typeof(IReflect));
            foreach (var binding in bindings)
            {
                if (binding == null)
                    throw new Exception();
            }
            GetBindingCache(_kernelWithConstructorAndPropertyInjection).Clear();
        }

        [Benchmark]
        public void GetBindings_Mixed()
        {
            _kernelWithConstructorAndPropertyInjection.GetBindings(typeof(IWarrior));
            _kernelWithConstructorAndPropertyInjection.GetBindings(typeof(IReflect));
            _kernelWithConstructorAndPropertyInjection.GetBindings(typeof(IWeapon));
            _kernelWithConstructorAndPropertyInjection.GetBindings(typeof(ICleric));
            GetBindingCache(_kernelWithConstructorAndPropertyInjection).Clear();
        }

        [Benchmark]
        public void GetOfT_ConstructorAndPropertyInjection_Transient_ResolveOnly_Singlethreaded()
        {
            _kernelWithConstructorAndPropertyInjection.Get<ICleric>();
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void GetOfT_ConstructorAndPropertyInjection_Transient_ResolveOnly_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < PerThreadLoopCount; i++)
                    {
                        _kernelWithConstructorAndPropertyInjection.Get<ICleric>();
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void GetOfT_ConstructorAndPropertyInjection_Singleton_ResolveOnly_Singlethreaded()
        {
            _kernelWithConstructorAndPropertyInjection.Get<ISingletonService>();
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void GetOfT_ConstructorAndPropertyInjection_Singleton_ResolveOnly_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < PerThreadLoopCount; i++)
                    {
                        _kernelWithConstructorAndPropertyInjection.Get<ISingletonService>();
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void GetOfT_OnlyConstructorInjection_Transient_ResolveOnly_Singlethreaded()
        {
            _kernelWithOnlyConstructorInjection.Get<ICleric>();
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void GetOfT_OnlyConstructorInjection_Transient_ResolveOnly_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < PerThreadLoopCount; i++)
                    {
                        _kernelWithOnlyConstructorInjection.Get<ICleric>();
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void GetOfT_OnlyConstructorInjection_Singleton_ResolveAndRelease_Singlethreaded()
        {
            var instance = _kernelWithOnlyConstructorInjection.Get<ISingletonService>();
            _kernelWithOnlyConstructorInjection.Release(instance);
        }

        [Benchmark]
        public void GetOfT_OnlyConstructorInjection_SingletonScope_ResolveOnly_Singlethreaded()
        {
            _kernelWithOnlyConstructorInjection.Get<ISingletonService>();
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void GetOfT_OnlyConstructorInjection_SingletonScope_ResolveOnly_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < PerThreadLoopCount; i++)
                    {
                        _kernelWithOnlyConstructorInjection.Get<ISingletonService>();
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void GetOfT_OnlyConstructorInjection_ThreadScope_ResolveAndRelease_Singlethreaded()
        {
            var instance = _kernelWithOnlyConstructorInjection.Get<IThreadLocalService>();
            _kernelWithOnlyConstructorInjection.Release(instance);
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void GetOfT_OnlyConstructorInjection_ThreadScope_ResolveAndRelease_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < PerThreadLoopCount; i++)
                    {
                        var instance = _kernelWithOnlyConstructorInjection.Get<IThreadLocalService>();
                        _kernelWithOnlyConstructorInjection.Release(instance);
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void GetOfT_OnlyConstructorInjection_ThreadScope_ResolveOnly_Singlethreaded()
        {
            _kernelWithOnlyConstructorInjection.Get<IThreadLocalService>();
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void GetOfT_OnlyConstructorInjection_ThreadScope_ResolveOnly_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < PerThreadLoopCount; i++)
                    {
                        _kernelWithOnlyConstructorInjection.Get<IThreadLocalService>();
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void GetOfT_ConstructorAndPropertyInjection_ThreadScope_ResolveAndRelease_Singlethreaded()
        {
            var instance = _kernelWithConstructorAndPropertyInjection.Get<IThreadLocalService>();
            _kernelWithConstructorAndPropertyInjection.Release(instance);
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void GetOfT_ConstructorAndPropertyInjection_ThreadScope_ResolveAndRelease_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < PerThreadLoopCount; i++)
                    {
                        var instance = _kernelWithConstructorAndPropertyInjection.Get<IThreadLocalService>();
                        _kernelWithConstructorAndPropertyInjection.Release(instance);
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        [Benchmark]
        public void GetOfT_ConstructorAndPropertyInjection_ThreadScope_ResolveOnly_Singlethreaded()
        {
            _kernelWithConstructorAndPropertyInjection.Get<IThreadLocalService>();
        }

        [Benchmark(OperationsPerInvoke = PerThreadLoopCount * ThreadCount)]
        public void GetOfT_ConstructorAndPropertyInjection_ThreadScope_ResolveOnly_Multithreaded()
        {
            var tasks = Enumerable.Range(1, ThreadCount)
                .Select(_ => Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < PerThreadLoopCount; i++)
                    {
                        _kernelWithConstructorAndPropertyInjection.Get<IThreadLocalService>();
                    }
                }, TaskCreationOptions.LongRunning));

            Task.WaitAll(tasks.ToArray());
        }

        public void Resolve_Array()
        {

        }

        public void Resolve_GenericType()
        {

        }

        [Benchmark]
        public void Resolve_Unique_MatchingBinding()
        {
            var instances = _kernelWithConstructorAndPropertyInjection.Resolve(_barracksRequest);
            if (instances == null)
                throw new Exception();

            using (var enumerator = instances.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new Exception();
                if (enumerator.MoveNext())
                    throw new Exception();
            }
        }

        [Benchmark]
        public void Resolve_Unique_MatchingBinding_RequestAndBindingParameters()
        {
            var instances = _kernelWithConstructorAndPropertyInjection.Resolve(_leasureRequest);
            if (instances == null)
                throw new Exception();

            using (var enumerator = instances.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new Exception();
                if (enumerator.MoveNext())
                    throw new Exception();
            }
        }

        [Benchmark]
        public void Resolve_NonUnique_MatchingBinding()
        {
            var instances = _kernelWithConstructorAndPropertyInjection.Resolve(_weaponRequest);
            if (instances == null)
                throw new Exception();

            using (var enumerator = instances.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    throw new Exception();
                if (enumerator.MoveNext())
                    throw new Exception();
            }
        }

        public void Resolve_MissingBinding()
        {
        }

        private static Dictionary<Type, IBinding[]> GetBindingCache(IReadOnlyKernel readOnlyKernel)
        {
            const string bindingCacheFieldName = "bindingCache";

            var bindingCacheField = typeof(ReadOnlyKernel).GetField(bindingCacheFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (bindingCacheField == null)
            {
                throw new Exception($"Field '{bindingCacheFieldName}' does not exist in {nameof(ReadOnlyKernel)}. Update {nameof(ReadOnlyKernelBenchmark) + "." + nameof(GetBindingCache)} to match the {nameof(ReadOnlyKernel)} implementation.");
            }

            var bindingCache = bindingCacheField.GetValue(readOnlyKernel) as Dictionary<Type, IBinding[]>;
            if (bindingCache == null)
            {
                throw new Exception($"BindingCache is null or has changed type. Expected type {typeof(Dictionary<Type, IBinding[]>).FullName}, but was {bindingCacheField.FieldType.FullName}.");
            }

            return bindingCache;
        }

        private static KernelConfiguration CreateKernelConfiguration(INinjectSettings ninjectSettings)
        {
            var kernelConfiguration = new KernelConfiguration(ninjectSettings);
            kernelConfiguration.Bind<IWarrior>().To<SpecialNinja>().WhenInjectedExactlyInto<NinjaBarracks>();
            kernelConfiguration.Bind<IWarrior>().To<Samurai>().WhenInjectedExactlyInto<Barracks>();
            kernelConfiguration.Bind<IWarrior>().To<FootSoldier>().WhenInjectedExactlyInto<Barracks>();
            kernelConfiguration.Bind<IWarrior>().To<FootSoldier>();
            kernelConfiguration.Bind<IWeapon>().To<Shuriken>().WhenInjectedExactlyInto<Barracks>();
            kernelConfiguration.Bind<IWeapon>().To<ShortSword>().WhenInjectedExactlyInto<NinjaBarracks>();
            kernelConfiguration.Bind<IWeapon>().To<Sword>();
            kernelConfiguration.Bind<IWeapon>().To<Dagger>();
            kernelConfiguration.Bind<ICleric>().To<Monk>();
            kernelConfiguration.Bind(typeof(ICollection<>)).To(typeof(List<>));
            kernelConfiguration.Bind(typeof(IList<>)).To(typeof(List<>));
            kernelConfiguration.Bind<ISingletonService>().To<SingletonService>().InSingletonScope();
            kernelConfiguration.Bind<IThreadLocalService>().To<ThreadLocalService>().InThreadScope();

            kernelConfiguration.Bind<ILeasure>()
                               .To<TakeAWalk>()
                               .WithParameter(new ConstructorArgument("walkTime", TimeSpan.FromMinutes(5), true));
            kernelConfiguration.Bind<IAnimal>()
                               .To<Dog>()
                               .WhenInjectedExactlyInto<TakeAWalk>();
            kernelConfiguration.Bind<IWeapon>()
                               .To<Dagger>()
                               .WhenInjectedExactlyInto<TakeAWalk>();

            return kernelConfiguration;
        }

        public interface ISingletonService
        {
        }

        public class SingletonService : ISingletonService
        {
            public SingletonService(ICleric cleric, IWarrior warrior)
            {
                this.Cleric = cleric;
                this.Warrior = warrior;
            }

            [Inject]
            public ICleric Cleric { get; set; }
            [Inject]
            public IWarrior Warrior { get; set; }
        }

        public interface IThreadLocalService
        {
        }

        public class ThreadLocalService : IThreadLocalService
        {
            public ThreadLocalService(ICleric cleric, IWarrior warrior)
            {
                this.Cleric = cleric;
                this.Warrior = warrior;
            }

            [Inject]
            public ICleric Cleric { get; set; }
            [Inject]
            public IWarrior Warrior { get; set; }
        }

        public class TakeAWalk : ILeasure
        {
            public TakeAWalk(IAnimal animal, IWarrior warrior, IWeapon weapon, TimeSpan walkTime, bool immediately)
            {
            }
        }

        public interface ILeasure
        {
        }

        public interface IAnimal
        {
            void Jump();
        }

        public class Dog : IAnimal
        {
            public void Jump()
            {
            }
        }
    }
}
