using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Hooks;
using Ninject.Activation.Providers;
using Ninject.Activation.Scope;
using Ninject.Components;
using Ninject.Events;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Disposal;
using Ninject.Infrastructure.Language;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace Ninject
{
	/// <summary>
	/// The base implementation of an <see cref="IKernel"/>.
	/// </summary>
	public abstract class KernelBase : DisposableObject, IKernel
	{
		private readonly Multimap<Type, IBinding> _bindings = new Multimap<Type, IBinding>();
		private readonly Dictionary<Type, IModule> _modules = new Dictionary<Type, IModule>();

		/// <summary>
		/// Gets the kernel settings.
		/// </summary>
		public INinjectSettings Settings { get; private set; }

		/// <summary>
		/// Gets the component container, which holds components that contribute to Ninject.
		/// </summary>
		public IComponentContainer Components { get; private set; }

		/// <summary>
		/// Occurs when a binding is added.
		/// </summary>
		public event EventHandler<BindingEventArgs> BindingAdded;

		/// <summary>
		/// Occurs when a binding is removed.
		/// </summary>
		public event EventHandler<BindingEventArgs> BindingRemoved;

		/// <summary>
		/// Occurs when a module is loaded.
		/// </summary>
		public event EventHandler<ModuleEventArgs> ModuleLoaded;

		/// <summary>
		/// Occurs when a module is unloaded.
		/// </summary>
		public event EventHandler<ModuleEventArgs> ModuleUnloaded;

		/// <summary>
		/// Initializes a new instance of the <see cref="KernelBase"/> class.
		/// </summary>
		protected KernelBase()
			: this(new ComponentContainer(), new NinjectSettings(), new IModule[0]) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="KernelBase"/> class.
		/// </summary>
		/// <param name="modules">The modules to load into the kernel.</param>
		protected KernelBase(IEnumerable<IModule> modules)
			: this(new ComponentContainer(), new NinjectSettings(), modules) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="KernelBase"/> class.
		/// </summary>
		/// <param name="settings">The configuration to use.</param>
		/// <param name="modules">The modules to load into the kernel.</param>
		protected KernelBase(INinjectSettings settings, IEnumerable<IModule> modules)
			: this(new ComponentContainer(), settings, modules) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="KernelBase"/> class.
		/// </summary>
		/// <param name="components">The component container to use.</param>
		/// <param name="settings">The configuration to use.</param>
		/// <param name="modules">The modules to load into the kernel.</param>
		protected KernelBase(IComponentContainer components, INinjectSettings settings, IEnumerable<IModule> modules)
		{
			Settings = settings;

			Components = components;
			components.Kernel = this;

			AddComponents();

			modules.Map(LoadModule);
		}

		/// <summary>
		/// Releases resources held by the object.
		/// </summary>
		public override void Dispose()
		{
			if (Components != null) Components.Dispose();
			base.Dispose();
		}

		/// <summary>
		/// Determines whether a module of the specified type has been loaded in the kernel.
		/// </summary>
		/// <param name="moduleType">The type of the module.</param>
		/// <returns><c>True</c> if the specified module has been loaded; otherwise, <c>false</c>.</returns>
		public virtual bool HasModule(Type moduleType)
		{
			return _modules.ContainsKey(moduleType);
		}

		/// <summary>
		/// Loads the module into the kernel.
		/// </summary>
		/// <param name="module">The module to load.</param>
		public virtual void LoadModule(IModule module)
		{
			_modules.Add(module.GetType(), module);
			module.OnLoad(this);

			ModuleLoaded.Raise(this, new ModuleEventArgs(module));
		}

		/// <summary>
		/// Unloads the module with the specified type.
		/// </summary>
		/// <param name="moduleType">The type of the module.</param>
		public virtual void UnloadModule(Type moduleType)
		{
			IModule module = _modules[moduleType];

			module.OnUnload(this);
			_modules.Remove(moduleType);

			ModuleUnloaded.Raise(this, new ModuleEventArgs(module));
		}

		/// <summary>
		/// Injects the specified existing instance, without managing its lifecycle.
		/// </summary>
		/// <param name="instance">The instance to inject.</param>
		public void Inject(object instance)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Determines whether the specified request can be resolved.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
		public bool CanResolve(IRequest request)
		{
			if (_bindings.ContainsKey(request.Service))
				return true;

			if (request.Service.IsGenericType && _bindings.ContainsKey(request.Service.GetGenericTypeDefinition()))
				return true;

			return false;
		}

		/// <summary>
		/// Resolves the specified request.
		/// </summary>
		/// <param name="service">The service to resolve.</param>
		/// <param name="constraints">The constraints to apply to the bindings to determine if they match the request.</param>
		/// <param name="parameters">The parameters to pass to the resolution.</param>
		/// <returns>A series of hooks that can be used to resolve instances that match the request.</returns>
		public virtual IEnumerable<IHook> Resolve(Type service, IEnumerable<Func<IBindingMetadata, bool>> constraints, IEnumerable<IParameter> parameters)
		{
			return Resolve(CreateDirectRequest(service, constraints, parameters));
		}

		/// <summary>
		/// Resolves the specified request.
		/// </summary>
		/// <param name="request">The request to resolve.</param>
		/// <returns>A series of hooks that can be used to resolve instances that match the request.</returns>
		public virtual IEnumerable<IHook> Resolve(IRequest request)
		{
			if (request.Service.IsAssignableFrom(GetType()))
				return new[] { new ConstantHook(this) };

			if (!CanResolve(request))
			{
				if (!TryRegisterImplicitSelfBinding(request.Service))
					throw new NotSupportedException(String.Format("No binding registered for {0}", request.Service)); 
			}

			return GetBindings(request)
				.Where(binding => binding.ConditionsSatisfiedBy(request) && request.ConstraintsSatisfiedBy(binding))
				.Select(binding => CreateHook(CreateContext(request, binding)));
		}

		/// <summary>
		/// Declares a binding for the specified service.
		/// </summary>
		/// <typeparam name="T">The service to bind.</typeparam>
		public IBindingToSyntax<T> Bind<T>()
		{
			return RegisterBindingAndCreateBuilder<T>(typeof(T));
		}

		/// <summary>
		/// Declares a binding for the specified service.
		/// </summary>
		/// <param name="service">The service to bind.</param>
		public IBindingToSyntax<object> Bind(Type service)
		{
			return RegisterBindingAndCreateBuilder<object>(service);
		}

		/// <summary>
		/// Registers the specified binding.
		/// </summary>
		/// <param name="binding">The binding to add.</param>
		public void AddBinding(IBinding binding)
		{
			_bindings.Add(binding.Service, binding);
			BindingAdded.Raise(this, new BindingEventArgs(binding));
		}

		/// <summary>
		/// Unregisters the specified binding.
		/// </summary>
		/// <param name="binding">The binding to remove.</param>
		public void RemoveBinding(IBinding binding)
		{
			_bindings.Remove(binding.Service, binding);
			BindingRemoved.Raise(this, new BindingEventArgs(binding));
		}

		/// <summary>
		/// Gets the bindings that match the request.
		/// </summary>
		/// <param name="request">The request to match.</param>
		/// <returns>A series of bindings that match the request.</returns>
		public virtual IEnumerable<IBinding> GetBindings(IRequest request)
		{
			foreach (IBinding binding in _bindings[request.Service])
				yield return binding;

			if (request.Service.IsGenericType)
			{
				Type gtd = request.Service.GetGenericTypeDefinition();

				foreach (IBinding binding in _bindings[gtd])
					yield return binding;
			}
		}

		/// <summary>
		/// Begins a new activation scope, which can be used to deterministically dispose resolved instances.
		/// </summary>
		/// <returns>The new activation scope.</returns>
		public IResolutionRoot BeginScope()
		{
			return new ActivationScope(this);
		}

		/// <summary>
		/// Adds components to the kernel during startup.
		/// </summary>
		protected abstract void AddComponents();

		/// <summary>
		/// Tries to register an implicit self-binding for the specified service.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns><c>True</c> if the type is self-bindable; otherwise <c>false</c>.</returns>
		protected virtual bool TryRegisterImplicitSelfBinding(Type service)
		{
			if (service.IsInterface || service.IsAbstract || service.ContainsGenericParameters)
				return false;

			var binding = new Binding(service) { ProviderCallback = StandardProvider.GetCreationCallback(service) };
			AddBinding(binding);

			return true;
		}

		/// <summary>
		/// Registers the specified binding and creates a builder to complete it.
		/// </summary>
		/// <typeparam name="T">The service being bound, or <see cref="object"/> if the non-generic version was used.</typeparam>
		/// <param name="service">The service being bound.</param>
		/// <returns>The builder that can be used to complete the binding.</returns>
		protected virtual BindingBuilder<T> RegisterBindingAndCreateBuilder<T>(Type service)
		{
			var binding = new Binding(service);
			AddBinding(binding);
			return new BindingBuilder<T>(binding);
		}

		/// <summary>
		/// Creates a request for the specified service.
		/// </summary>
		/// <param name="service">The service to resolve.</param>
		/// <param name="constraints">The constraints to apply to the bindings to determine if they match the request.</param>
		/// <param name="parameters">The parameters to pass to the resolution.</param>
		/// <returns>The created request.</returns>
		protected virtual IRequest CreateDirectRequest(Type service, IEnumerable<Func<IBindingMetadata, bool>> constraints, IEnumerable<IParameter> parameters)
		{
			return new Request(service, constraints, parameters, null);
		}

		/// <summary>
		/// Creates a context for the specified request and binding.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="binding">The binding.</param>
		/// <returns>The created context.</returns>
		protected virtual IContext CreateContext(IRequest request, IBinding binding)
		{
			return new Context(this, request, binding);
		}

		/// <summary>
		/// Creates a hook that can resolve the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created hook.</returns>
		protected virtual IHook CreateHook(IContext context)
		{
			return new ContextResolutionHook(context, Components.Get<ICache>());
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			return this.Get(serviceType);
		}
	}
}
