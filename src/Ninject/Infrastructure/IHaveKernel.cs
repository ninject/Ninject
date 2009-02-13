using System;

namespace Ninject.Infrastructure
{
	/// <summary>
	/// Indicates that the object has a reference to an <see cref="IKernel"/>.
	/// </summary>
	public interface IHaveKernel
	{
		/// <summary>
		/// Gets the kernel.
		/// </summary>
		IKernel Kernel { get; }
	}
}