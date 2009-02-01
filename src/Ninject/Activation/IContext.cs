using System;
using Ninject.Bindings;
using Ninject.Creation;
using Ninject.Planning;

namespace Ninject.Activation
{
	public interface IContext
	{
		IKernel Kernel { get; }
		IRequest Request { get; }
		IBinding Binding { get; }
		IPlan Plan { get; set; }
		IProvider Provider { get; }
		Type Implementation { get; }
		object Instance { get; set; }

		object GetScope();
		object Resolve();
	}
}