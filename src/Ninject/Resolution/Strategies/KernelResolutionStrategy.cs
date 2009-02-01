using System;
using Ninject.Activation;

namespace Ninject.Resolution.Strategies
{
	public class KernelResolutionStrategy : IResolutionStrategy
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