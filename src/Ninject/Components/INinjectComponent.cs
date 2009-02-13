using System;

namespace Ninject.Components
{
	/// <summary>
	/// A component that contributes to Ninject.
	/// </summary>
	public interface INinjectComponent : IDisposable
	{
		/// <summary>
		/// Gets or sets the kernel that the component is attached to.
		/// </summary>
		IKernel Kernel { get; set; }
	}
}