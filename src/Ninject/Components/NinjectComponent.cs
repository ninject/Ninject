using System;
using Ninject.Infrastructure.Disposal;

namespace Ninject.Components
{
	/// <summary>
	/// An abstract component that contributes to Ninject.
	/// </summary>
	public abstract class NinjectComponent : DisposableObject, INinjectComponent
	{
		/// <summary>
		/// Gets or sets the kernel that the component is attached to.
		/// </summary>
		public IKernel Kernel { get; set; }
	}
}