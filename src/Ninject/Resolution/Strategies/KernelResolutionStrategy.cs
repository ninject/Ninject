using System;
using Ninject.Activation;
using Ninject.Components;

namespace Ninject.Resolution.Strategies
{
	public class KernelResolutionStrategy : NinjectComponent, IResolutionStrategy
	{
		public bool Supports(IRequest request)
		{
			return typeof(IKernel).IsAssignableFrom(request.Service);
		}

		public object Resolve(IContext context)
		{
			return context.Kernel;
		}
	}
}