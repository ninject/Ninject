using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Events;
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

		event EventHandler<ModuleEventArgs> ModuleLoaded;
		event EventHandler<ModuleEventArgs> ModuleUnloaded;

		bool HasModule(Type moduleType);
		void LoadModule(IModule module);
		void UnloadModule(Type moduleType);

		void Inject(object instance);
		IEnumerable<IBinding> GetBindings(IRequest request);

		IResolutionRoot BeginScope();
	}
}
