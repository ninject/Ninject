using System;
using Ninject.Components;

namespace Ninject.Activation.Caching
{
	public interface ICache : INinjectComponent
	{
		void Remember(IContext context);
		object TryGet(IContext context);
		void Prune();
	}
}