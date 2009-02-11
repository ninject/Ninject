using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Activation.Hooks;
using Ninject.Parameters;
using Ninject.Planning.Bindings;

namespace Ninject.Syntax
{
	public interface IResolutionRoot : IServiceProvider
	{
		bool CanResolve(IRequest request);
		IEnumerable<IHook> Resolve(Type service, IEnumerable<Func<IBindingMetadata, bool>> constraints, IEnumerable<IParameter> parameters);
		IEnumerable<IHook> Resolve(IRequest request);
	}
}