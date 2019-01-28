using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Strategies;
using Ninject.Components;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Planning.Strategies;
using Ninject.Planning.Targets;
using Ninject.Tests.Fakes;

namespace Ninject.Benchmarks.Activation
{
    [MemoryDiagnoser]
    public class RequestBenchmark
    {
        private Type _service;
        private Func<IBindingMetadata, bool> _constraint;
        private IParameter[] _emptyParameters;
        private List<IParameter> _requestParameterWithMixOfShouldInheritSetToTrueAndFalse;
        private List<IParameter> _bindingParameterWithMixOfShouldInheritSetToTrueAndFalse;
        private Func<object> _scopeCallback;
        private GarbageCollectionCachePruner _cachePruner;
        private Pipeline _pipeline;
        private Cache _cache;
        private Planner _planner;
        private ExceptionFormatter _exceptionFormatter;
        private IRequest _rootRequestWithMixOfShouldInheritSetToTrueAndFalse;
        private IRequest _childRequestWithoutParameters;
        private IRequest _childRequestWithRequestAndBindingParameters;
        private IRequest _rootRequestWithoutParameters;
        private Context _contextWithoutRequestAndBindingParameters;
        private IBinding _bindingWithParameters;
        private Context _contextWithRequestAndBindingParameters;
        private ITarget _target;
        private IBinding _bindingWithoutParameters;
        private NinjectSettings _ninjectSettings;
        private KernelConfiguration _kernelConfiguration;
        private IReadOnlyKernel _kernel;

        public RequestBenchmark()
        {
            _ninjectSettings = new NinjectSettings
                {
                    // Disable to reduce memory pressure
                    ActivationCacheDisabled = true,
                    LoadExtensions = false
                };
            _kernelConfiguration = new KernelConfiguration(_ninjectSettings);
            _kernel = _kernelConfiguration.BuildReadOnlyKernel();
            _service = typeof(MyInstrumentedService);
            _target = CreatePropertyTarget(_service);
            _constraint = null;
            _scopeCallback = null;
            _cachePruner = new GarbageCollectionCachePruner(_ninjectSettings);
            _pipeline = new Pipeline(Enumerable.Empty<IActivationStrategy>(), new ActivationCache(_cachePruner));
            _cache = new Cache(_pipeline, _cachePruner);
            _planner = new Planner(Enumerable.Empty<IPlanningStrategy>());
            _exceptionFormatter = new ExceptionFormatter();

            _emptyParameters = Array.Empty<IParameter>();
            _requestParameterWithMixOfShouldInheritSetToTrueAndFalse = new List<IParameter>
                {
                        new ConstructorArgument("foo1", 1, true),
                        new ConstructorArgument("foo3", 3, true),
                        new ConstructorArgument("foo3", 3, false),
                        new ConstructorArgument("foo5", 5, false)
                };
            _bindingParameterWithMixOfShouldInheritSetToTrueAndFalse = new List<IParameter>
                {
                        new ConstructorArgument("foo1", 1, true),
                        new ConstructorArgument("foo2", 2, true),
                        new ConstructorArgument("foo3", 3, false),
                        new ConstructorArgument("foo4", 4, false),
                };

            _rootRequestWithoutParameters = CreateRootRequest(_emptyParameters);
            _rootRequestWithMixOfShouldInheritSetToTrueAndFalse = CreateRootRequest(_requestParameterWithMixOfShouldInheritSetToTrueAndFalse);

            _bindingWithoutParameters = CreateBinding(Array.Empty<IParameter>());
            _contextWithoutRequestAndBindingParameters = CreateContext(_rootRequestWithoutParameters, _bindingWithoutParameters);
            _bindingWithParameters = CreateBinding(_bindingParameterWithMixOfShouldInheritSetToTrueAndFalse);
            _contextWithRequestAndBindingParameters = CreateContext(_rootRequestWithMixOfShouldInheritSetToTrueAndFalse, _bindingWithParameters);

            _childRequestWithoutParameters = CreateChildRequest(_contextWithoutRequestAndBindingParameters, _target);
            _childRequestWithRequestAndBindingParameters = CreateChildRequest(_contextWithRequestAndBindingParameters, _target);
        }

        [Benchmark]
        public void Constructor_ServiceAndConstraintAndParametersAndScopeCallbackAndIsOptionalAndIsUnique_ParameterEmpty()
        {
            new Request(_service, _constraint, _emptyParameters, _scopeCallback, false, true);
        }

        [Benchmark]
        public void Constructor_ServiceAndConstraintAndParametersAndScopeCallbackAndIsOptionalAndIsUnique_MixOfParametersWithShouldInheritSetToTrueAndFalse()
        {
            new Request(_service, _constraint, _bindingParameterWithMixOfShouldInheritSetToTrueAndFalse, _scopeCallback, false, true);
        }

        [Benchmark]
        public void Constructor_ParentContextAndServiceAndTargetAndScopeCallback_ParentContextWithRequestAndBindingParameters()
        {
            new Request(_contextWithRequestAndBindingParameters, _service, _target, _scopeCallback);
        }

        [Benchmark]
        public void Constructor_ParentContextAndServiceAndTargetAndScopeCallback_ParentContextWithoutRequestAndBindingParameters()
        {
            new Request(_contextWithoutRequestAndBindingParameters, _service, _target, _scopeCallback);
        }

        [Benchmark]
        public void Parameters_RootRequest_ParameterEmpty()
        {
            foreach (var param in _rootRequestWithoutParameters.Parameters)
            {
                if (param == null)
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void Parameters_RootRequest_MixOfParametersWithShouldInheritSetToTrueAndFalse()
        {
            foreach (var param in _rootRequestWithMixOfShouldInheritSetToTrueAndFalse.Parameters)
            {
                if (param == null)
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void Parameters_ChildRequest_ParentContextWithRequestAndBindingParameters()
        {
            foreach (var param in _childRequestWithRequestAndBindingParameters.Parameters)
            {
                if (param == null)
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void Parameters_ChildRequest_ParentContextWithoutRequestAndBindingParameters()
        {
            foreach (var param in _childRequestWithoutParameters.Parameters)
            {
                if (param == null)
                {
                    throw new Exception();
                }
            }
        }

        private IRequest CreateRootRequest(IReadOnlyList<IParameter> parameters)
        {
            return new Request(GetType(), null, parameters, null, false, true);
        }

        private IRequest CreateChildRequest(IContext parentContext, ITarget target)
        {
            return new Request(parentContext, GetType(), target, null);
        }

        private IBinding CreateBinding(IEnumerable<IParameter> parameters)
        {
            var binding = new Binding(GetType());
            foreach (var param in parameters)
            {
                binding.Parameters.Add(param);
            }
            return binding;
        }

        private Context CreateContext(IRequest request, IBinding binding)
        {
            return new Context(_kernel, _ninjectSettings, request, binding, _cache, _planner, _pipeline, _exceptionFormatter);
        }

        private static ITarget CreatePropertyTarget(Type service)
        {
            var property = service.GetProperty(nameof(MyInstrumentedService.Warrior));
            if (property == null)
            {
                throw new Exception($"{nameof(MyInstrumentedService.Warrior)} is no longer a property. Please update the {nameof(RequestBenchmark)} class.");
            }

            return new PropertyTarget(property);
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
    }
}
