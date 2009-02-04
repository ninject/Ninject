using System;
using System.Collections.Generic;
using Ninject.Activation.Constraints;
using Ninject.Infrastructure.Disposal;
using Ninject.Parameters;
using Ninject.Syntax;

namespace Ninject.Activation.Scope
{
	public class ActivationScope : DisposableObject, IActivationScope
	{
		private readonly IResolutionRoot _parent;

		public ActivationScope(IResolutionRoot parent)
		{
			_parent = parent;
		}

		public bool CanResolve(IRequest request)
		{
			return _parent.CanResolve(request);
		}

		public IEnumerable<IContext> Resolve(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters)
		{
			return Resolve(CreateDirectRequest(service, constraints, parameters));
		}

		public IEnumerable<IContext> Resolve(IRequest request)
		{
			return _parent.Resolve(request);
		}

		protected virtual IRequest CreateDirectRequest(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters)
		{
			return new Request(service, constraints, parameters, () => this);
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			return this.Get(serviceType);
		}
	}
}