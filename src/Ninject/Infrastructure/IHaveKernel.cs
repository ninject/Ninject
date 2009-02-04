using System;

namespace Ninject.Infrastructure
{
	public interface IHaveKernel
	{
		IKernel Kernel { get; }
	}
}