using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure.Disposal;
using Ninject.Messaging.Messages;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject
{
	public interface IKernel : IBindingRoot, IResolutionRoot, INotifyWhenDisposed
	{
		INinjectSettings Settings { get; }
		IComponentContainer Components { get; }

		event EventHandler<ModuleMessage> ModuleLoaded;
		event EventHandler<ModuleMessage> ModuleUnloaded;

		void Load(IModule module);
		void Unload(string moduleName);
		void Unload(IModule module);

		void Inject(object instance);
		IEnumerable<IBinding> GetBindings(IRequest request);

		IResolutionRoot BeginScope();
	}
}
