using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation.Caching;
using Ninject.Infrastructure.Tracing;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;

namespace Ninject.Activation
{
	public class Context : TraceInfoProvider, IContext
	{
		public IKernel Kernel { get; set; }
		public IRequest Request { get; set; }
		public IBinding Binding { get; set; }
		public IPlan Plan { get; set; }
		public ICollection<IParameter> Parameters { get; set; }
		public object Instance { get; set; }

		public Type[] GenericArguments { get; private set; }
		public bool HasInferredGenericArguments { get; private set; }

		public Context(IKernel kernel, IRequest request, IBinding binding)
		{
			Kernel = kernel;
			Request = request;
			Binding = binding;
			Parameters = request.Parameters.Union(binding.Parameters).ToList();

			if (binding.Service.IsGenericTypeDefinition)
			{
				HasInferredGenericArguments = true;
				GenericArguments = request.Service.GetGenericArguments();
			}
		}

		public object GetScope()
		{
			return Request.GetScope() ?? Binding.GetScope(this);
		}

		public IProvider GetProvider()
		{
			return Binding.GetProvider(this);
		}
	}
}