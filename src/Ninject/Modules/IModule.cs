using System;
using Ninject.Syntax;

namespace Ninject.Modules
{
	public interface IModule : IBindingRoot
	{
		IKernel Kernel { get; set; }

		void OnLoad(IKernel kernel);
		void OnUnload(IKernel kernel);

		void Load();
		void Unload();
	}
}