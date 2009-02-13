using System;
using Ninject.Syntax;

namespace Ninject.Modules
{
	/// <summary>
	/// A pluggable unit that can be loaded into a kernel.
	/// </summary>
	public interface IModule : IBindingRoot
	{
		/// <summary>
		/// Gets or sets the kernel that the module is loaded into.
		/// </summary>
		IKernel Kernel { get; set; }

		/// <summary>
		/// Called when the module is loaded into a kernel.
		/// </summary>
		/// <param name="kernel">The kernel that is loading the module.</param>
		void OnLoad(IKernel kernel);

		/// <summary>
		/// Called when the module is unloaded from a kernel.
		/// </summary>
		/// <param name="kernel">The kernel that is unloading the module.</param>
		void OnUnload(IKernel kernel);
	}
}