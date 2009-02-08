using System;

namespace Ninject.Components
{
	public interface INinjectComponent : IDisposable
	{
		IKernel Kernel { get; set; }
	}
}