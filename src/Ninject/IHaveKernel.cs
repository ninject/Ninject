using System;

namespace Ninject
{
	public interface IHaveKernel
	{
		IKernel Kernel { get; }
	}
}
