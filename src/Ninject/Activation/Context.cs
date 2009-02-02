using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Creation;
using Ninject.Infrastructure.Tracing;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Resolution;

namespace Ninject.Activation
{
	public class Context : TraceInfoProvider, IContext
	{
		private IProvider _provider;
		private Type _implementation;

		public IKernel Kernel { get; set; }
		public IRequest Request { get; set; }
		public IBinding Binding { get; set; }
		public IPlan Plan { get; set; }
		public ICollection<IParameter> Parameters { get; set; }
		public object Instance { get; set; }

		public IResolver Resolver { get; set; }

		public IProvider Provider
		{
			get
			{
				if (_provider == null) _provider = Binding.GetProvider(this);
				return _provider;
			}
		}

		public Type Implementation
		{
			get
			{
				if (_implementation == null) _implementation = Provider.GetImplementationType(this);
				return _implementation;
			}
		}

		public object GetScope()
		{
			return Request.GetScope() ?? Binding.GetScope(this);
		}

		public Context(IKernel kernel, IRequest request, IBinding binding, IResolver resolver)
		{
			Kernel = kernel;
			Request = request;
			Binding = binding;
			Resolver = resolver;
			Parameters = request.Parameters.Union(binding.Parameters).ToList();
		}

		public object Resolve()
		{
			return Resolver.Resolve(this);
		}
	}
}