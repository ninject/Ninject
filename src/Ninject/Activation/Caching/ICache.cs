using System;
using Ninject.Infrastructure.Components;

namespace Ninject.Activation.Caching
{
	public interface ICache : INinjectComponent
	{
		void Remember(IContext context);
		object TryGet(Type type, object scope);
		void Prune();
	}
}