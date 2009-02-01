using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Resolution;

namespace Ninject.Infrastructure
{
	public interface IResolutionRoot
	{
		IEnumerable<IContext> Resolve(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters);
		IEnumerable<IContext> Resolve(IRequest request);
	}
}