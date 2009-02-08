using System;
using Ninject.Activation;

namespace Ninject.Infrastructure
{
	public class KernelHook : IHook
	{
		public IKernel Kernel { get; private set; }

		public KernelHook(IKernel kernel)
		{
			Kernel = kernel;
		}

		public object Resolve()
		{
			return Kernel;
		}
	}
}