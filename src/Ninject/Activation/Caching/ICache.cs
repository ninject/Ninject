using System;
using Ninject.Components;
using Ninject.Planning.Bindings;

namespace Ninject.Activation.Caching
{
	public interface ICache : INinjectComponent
	{
		void Remember(IContext context);
		object TryGet(IBinding binding, object scope);
		void Prune();
	}
}