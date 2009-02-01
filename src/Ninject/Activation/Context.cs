using System;
using Ninject.Bindings;
using Ninject.Creation;
using Ninject.Planning;
using Ninject.Resolution;

namespace Ninject.Activation
{
	public class Context : IContext
	{
		private IProvider _provider;
		private Type _implementation;

		public IKernel Kernel { get; set; }
		public IRequest Request { get; set; }
		public IBinding Binding { get; set; }
		public IPlan Plan { get; set; }
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
		}

		public object Resolve()
		{
			return Resolver.Resolve(this);
		}
	}
}