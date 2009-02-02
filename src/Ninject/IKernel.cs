using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure.Disposal;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

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
