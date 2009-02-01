using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Resolution;

namespace Ninject.Infrastructure
{
	public class ResolutionScope : DisposableObject, IResolutionScope
	{
		private readonly IResolutionRoot _parent;

		public ResolutionScope(IResolutionRoot parent)
		{
			_parent = parent;
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
	}
}