using System;
using Ninject.Activation;
using Ninject.Creation;

namespace Ninject.Bindings
{
	public interface IBinding
	{
		Type Service { get; }
		IBindingMetadata Metadata { get; }

		IProvider GetProvider(IContext context);
		object GetScope(IContext context);
		bool Matches(IRequest request);
	}
}