using System;
using System.Collections.Generic;
using Ninject.Infrastructure.Tracing;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;

namespace Ninject.Activation
{
	public interface IContext : IHaveTraceInfo
	{
		IKernel Kernel { get; }
		IRequest Request { get; }
		IBinding Binding { get; }
		IPlan Plan { get; set; }
		ICollection<IParameter> Parameters { get; }
		object Instance { get; set; }

		IProvider GetProvider();
		object GetScope();
		object Resolve();
	}
}