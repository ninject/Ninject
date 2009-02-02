using System;
using Ninject.Activation;
using Ninject.Components;

namespace Ninject.Resolution.Strategies
{
	public class ComponentResolutionStrategy : NinjectComponent, IResolutionStrategy
	{
		public bool Supports(IRequest request)
		{
			return typeof(INinjectComponent).IsAssignableFrom(request.Service);
		}

		public object Resolve(IContext context)
		{
			return context.Kernel.Components.Get(context.Request.Service);
		}
	}
}