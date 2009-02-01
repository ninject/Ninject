using System;
using Ninject.Activation;
using Ninject.Creation;

namespace Ninject.Bindings
{
	public interface IBinding
	{
		Type Service { get; }
		string Name { get; }

		object GetMetadata(string key);
		void SetMetadata(string key, object value);

		IProvider GetProvider(IContext context);
		object GetScope(IContext context);
		bool Matches(IRequest request);
	}
}