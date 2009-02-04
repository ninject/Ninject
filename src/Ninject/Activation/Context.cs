using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Infrastructure.Tracing;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;

namespace Ninject.Activation
{
	public class Context : TraceInfoProvider, IContext
	{
		private readonly Func<IContext, object> _resolveCallback;

		public IKernel Kernel { get; set; }
		public IRequest Request { get; set; }
		public IBinding Binding { get; set; }
		public IPlan Plan { get; set; }
		public ICollection<IParameter> Parameters { get; set; }
		public object Instance { get; set; }

		public Context(IKernel kernel, IRequest request, IBinding binding, Func<IContext, object> resolveCallback)
		{
			Kernel = kernel;
			Request = request;
			Binding = binding;
			Parameters = request.Parameters.Union(binding.Parameters).ToList();
			_resolveCallback = resolveCallback;
		}

		public object GetScope()
		{
			return Request.GetScope() ?? Binding.GetScope(this);
		}

		public IProvider GetProvider()
		{
			return Binding.GetProvider(this);
		}

		public object Resolve()
		{
			return _resolveCallback(this);
		}
	}
}