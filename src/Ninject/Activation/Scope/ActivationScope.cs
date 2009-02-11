using System;
using System.Collections.Generic;
using Ninject.Activation.Hooks;
using Ninject.Infrastructure.Disposal;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject.Activation.Scope
{
	public class ActivationScope : DisposableObject, IActivationScope
	{
		public IResolutionRoot Parent { get; private set; }

		public ActivationScope(IResolutionRoot parent)
		{
			Parent = parent;
		}

		public bool CanResolve(IRequest request)
		{
			return Parent.CanResolve(request);
		}

		public IEnumerable<IHook> Resolve(Type service, IEnumerable<Func<IBindingMetadata, bool>> constraints, IEnumerable<IParameter> parameters)
		{
			return Resolve(CreateDirectRequest(service, constraints, parameters));
		}

		public IEnumerable<IHook> Resolve(IRequest request)
		{
			return Parent.Resolve(request);
		}

		protected virtual IRequest CreateDirectRequest(Type service, IEnumerable<Func<IBindingMetadata, bool>> constraints, IEnumerable<IParameter> parameters)
		{
			return new Request(service, constraints, parameters, () => this);
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			return this.Get(serviceType);
		}
	}
}