using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Infrastructure.Tracing;
using Ninject.Parameters;

namespace Ninject.Planning.Bindings
{
	public interface IBinding : IHaveTraceInfo
	{
		Type Service { get; }
		IBindingMetadata Metadata { get; }

		ICollection<Func<IRequest, bool>> Conditions { get; }
		ICollection<IParameter> Parameters { get; }

		IProvider GetProvider(IContext context);
		object GetScope(IContext context);
		bool ConditionsSatisfiedBy(IRequest request);
	}
}