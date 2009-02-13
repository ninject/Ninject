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
	/// <summary>
	/// A super-factory that can create objects of all kinds, following hints provided by <see cref="IBinding"/>s.
	/// </summary>
	public interface IKernel : IBindingRoot, IResolutionRoot, INotifyWhenDisposed
	{
		/// <summary>
		/// Gets the kernel settings.
		/// </summary>
		INinjectSettings Settings { get; }

		/// <summary>
		/// Gets the component container, which holds components that contribute to Ninject.
		/// </summary>
		IComponentContainer Components { get; }

		/// <summary>
		/// Occurs when a module is loaded.
		/// </summary>
		event EventHandler<ModuleEventArgs> ModuleLoaded;

		/// <summary>
		/// Occurs when a module is unloaded.
		/// </summary>
		event EventHandler<ModuleEventArgs> ModuleUnloaded;

		/// <summary>
		/// Determines whether a module of the specified type has been loaded in the kernel.
		/// </summary>
		/// <param name="moduleType">The type of the module.</param>
		/// <returns><c>True</c> if the specified module has been loaded; otherwise, <c>false</c>.</returns>
		bool HasModule(Type moduleType);

		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		/// <param name="module">The module to load.</param>
		void LoadModule(IModule module);

		/// <summary>
		/// Unloads the module with the specified type.
		/// </summary>
		/// <param name="moduleType">The type of the module.</param>
		void UnloadModule(Type moduleType);

		/// <summary>
		/// Injects the specified existing instance, without managing its lifecycle.
		/// </summary>
		/// <param name="instance">The instance to inject.</param>
		void Inject(object instance);

		/// <summary>
		/// Gets the bindings that match the request.
		/// </summary>
		/// <param name="request">The request to match.</param>
		/// <returns>A series of bindings that match the request.</returns>
		IEnumerable<IBinding> GetBindings(IRequest request);

		/// <summary>
		/// Begins a new activation scope, which can be used to deterministically dispose resolved instances.
		/// </summary>
		/// <returns>The new activation scope.</returns>
		IResolutionRoot BeginScope();
	}
}
