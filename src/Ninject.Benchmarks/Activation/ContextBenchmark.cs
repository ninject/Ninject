using BenchmarkDotNet.Attributes;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Components;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using System;
using System.Collections.Generic;

namespace Ninject.Benchmarks.Activation
{
    [MemoryDiagnoser]
    public class ContextBenchmark
    {
        private IRequest _requestWithParameters;
        private IRequest _requestWithoutParameters;
        private IBinding _bindingWithParameters;
        private IBinding _bindingWithoutParameters;
        private IKernelConfiguration _kernelConfiguration;
        private IReadOnlyKernel _kernel;
        private ICache _cache;
        private IPlanner _planner;
        private IPipeline _pipeline;
        private IExceptionFormatter _exceptionFormatter;
        private Context _contextWithRequestParameters;
        private Context _contextWithBindingParameters;
        private Context _contextWithRequestAndBindingParameters;
        private Context _contextWithoutRequestAndBindingParameters;
        private NinjectSettings _ninjectSettings;

        public ContextBenchmark()
        {
            var requestParameters = new List<IParameter>
                {
                    new ConstructorArgument("foo", 5),
                    new ConstructorArgument("boo", 7),
                    new PropertyValue("name", "Tom")
                };

            var bindingParameters = new List<IParameter>
                {
                    new ConstructorArgument("abc", 2),
                    new ConstructorArgument("def", 1),
                    new PropertyValue("ghi", "Dirk")
                };

            _requestWithParameters = CreateRequest(requestParameters);
            _requestWithoutParameters = CreateRequest(Array.Empty<IParameter>());

            _bindingWithParameters = CreateBinding(bindingParameters);
            _bindingWithoutParameters = CreateBinding(Array.Empty<IParameter>());

            _ninjectSettings = new NinjectSettings
                {
                    // Disable to reduce memory pressure
                    ActivationCacheDisabled = true,
                    LoadExtensions = false
                };
            _kernelConfiguration = new KernelConfiguration(_ninjectSettings);
            _kernel = _kernelConfiguration.BuildReadOnlyKernel();
            _cache = _kernelConfiguration.Components.Get<ICache>();
            _planner = _kernelConfiguration.Components.Get<IPlanner>();
            _pipeline = _kernelConfiguration.Components.Get<IPipeline>();
            _exceptionFormatter = _kernelConfiguration.Components.Get<IExceptionFormatter>();

            _contextWithRequestParameters = CreateContext(_requestWithParameters, _bindingWithoutParameters);
            _contextWithBindingParameters = CreateContext(_requestWithoutParameters, _bindingWithParameters);
            _contextWithRequestAndBindingParameters = CreateContext(_requestWithParameters, _bindingWithParameters);
            _contextWithoutRequestAndBindingParameters = CreateContext(_requestWithoutParameters, _bindingWithoutParameters);
        }

        [Benchmark]
        public void Constructor_RequestWithParameters()
        {
            new Context(_kernel, _ninjectSettings, _requestWithParameters, _bindingWithoutParameters, 
                        _cache, _planner, _pipeline, _exceptionFormatter);
        }

        [Benchmark]
        public void Constructor_BindingWithParameters()
        {
            new Context(_kernel, _ninjectSettings, _requestWithoutParameters, _bindingWithParameters,
                        _cache, _planner, _pipeline, _exceptionFormatter);
        }

        [Benchmark]
        public void Constructor_RequestAndBindingWithParameters()
        {
            new Context(_kernel, _ninjectSettings, _requestWithParameters, _bindingWithParameters,
                        _cache, _planner, _pipeline, _exceptionFormatter);
        }

        [Benchmark]
        public void Constructor_RequestAndBindingWithoutParameters()
        {
            new Context(_kernel, _ninjectSettings, _requestWithoutParameters, _bindingWithoutParameters,
                        _cache, _planner, _pipeline, _exceptionFormatter);
        }

        [Benchmark]
        public void Parameters_RequestWithParameters()
        {
            foreach (var param in _contextWithRequestParameters.Parameters)
            {
                if (param == null)
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void Parameters_RequestAndBindingWithParameters()
        {
            foreach (var param in _contextWithRequestAndBindingParameters.Parameters)
            {
                if (param == null)
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void Parameters_RequestAndBindingWithoutParameters()
        {
            foreach (var param in _contextWithoutRequestAndBindingParameters.Parameters)
            {
                if (param == null)
                {
                    throw new Exception();
                }
            }
        }

        [Benchmark]
        public void Parameters_BindingWithParameters()
        {
            foreach (var param in _contextWithBindingParameters.Parameters)
            {
                if (param == null)
                {
                    throw new Exception();
                }
            }
        }

        private IRequest CreateRequest(IReadOnlyList<IParameter> parameters)
        { 
            return new Request(GetType(), null, parameters, null, false, true);
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
    }
}
