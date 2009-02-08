using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Activation.Constraints;
using Ninject.Parameters;

namespace Ninject.Syntax
{
	public interface IResolutionRoot : IServiceProvider
	{
		bool CanResolve(IRequest request);
		IEnumerable<IHook> Resolve(Type service, IEnumerable<IConstraint> constraints, IEnumerable<IParameter> parameters);
		IEnumerable<IHook> Resolve(IRequest request);
	}
}