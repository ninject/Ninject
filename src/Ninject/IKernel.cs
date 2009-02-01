using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Bindings;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Components;
using Ninject.Modules;

namespace Ninject
{
	public interface IKernel : IBindingRoot, IResolutionRoot, INotifyWhenDisposed
	{
		INinjectSettings Settings { get; }
		IComponentContainer Components { get; }

		void Load(IModule module);
		void Unload(string moduleName);
		void Unload(IModule module);

		IEnumerable<IBinding> GetBindings(IRequest request);

		IResolutionRoot BeginScope();
	}
}
