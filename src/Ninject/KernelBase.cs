#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Activation;
using Ninject.Activation.Blocks;
using Ninject.Activation.Caching;
using Ninject.Activation.Providers;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Infrastructure.Language;
using Ninject.Modules;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;
using Ninject.Syntax;
#endregion

namespace Ninject
{
	/// <summary>
	/// The base implementation of an <see cref="IKernel"/>.
	/// </summary>
	public abstract class KernelBase : BindingRoot, IKernel
	{
		private readonly Multimap<Type, IBinding> _bindings = new Multimap<Type, IBinding>();
		private readonly Multimap<Type, IBinding> _bindingCache = new Multimap<Type, IBinding>();
		private readonly Dictionary<string, INinjectModule> _modules = new Dictionary<string, INinjectModule>();

		/// <summary>
		/// Gets the kernel settings.
		/// </summary>
		public INinjectSettings Settings { get; private set; }

		/// <summary>
		/// Gets the component container, which holds components that contribute to Ninject.
		/// </summary>
		public IComponentContainer Components { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="KernelBase"/> class.
		/// </summary>
		protected KernelBase()
			: this(new ComponentContainer(), new NinjectSettings(), new INinjectModule[0]) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="KernelBase"/> class.
		/// </summary>
		/// <param name="modules">The modules to load into the kernel.</param>
		protected KernelBase(params INinjectModule[] modules)
			: this(new ComponentContainer(), new NinjectSettings(), modules) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="KernelBase"/> class.
		/// </summary>
		/// <param name="settings">The configuration to use.</param>
		/// <param name="modules">The modules to load into the kernel.</param>
		protected KernelBase(INinjectSettings settings, params INinjectModule[] modules)
			: this(new ComponentContainer(), settings, modules) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="KernelBase"/> class.
		/// </summary>
		/// <param name="components">The component container to use.</param>
		/// <param name="settings">The configuration to use.</param>
		/// <param name="modules">The modules to load into the kernel.</param>
		protected KernelBase(IComponentContainer components, INinjectSettings settings, params INinjectModule[] modules)
		{
			Ensure.ArgumentNotNull(components, "components");
			Ensure.ArgumentNotNull(settings, "settings");
			Ensure.ArgumentNotNull(modules, "modules");

			Settings = settings;

			Components = components;
			components.Kernel = this;

			AddComponents();

			#if !NO_WEB
			OnePerRequestModule.StartManaging(this);
			#endif

			#if !NO_ASSEMBLY_SCANNING
			if (Settings.LoadExtensions)
				Load(new[] { Settings.ExtensionSearchPattern });
			#endif

			Load(modules);
		}

		/// <summary>
		/// Releases resources held by the object.
		/// </summary>
		public override void Dispose(bool disposing)
		{
			if (disposing && !IsDisposed)
			{
				#if !NO_WEB
				OnePerRequestModule.StopManaging(this);
				#endif

				if (Components != null)
				{
					// Deactivate all cached instances before shutting down the kernel.
					var cache = Components.Get<ICache>();
					cache.Clear();

					Components.Dispose();
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Unregisters all bindings for the specified service.
		/// </summary>
		/// <param name="service">The service to unbind.</param>
		public override void Unbind(Type service)
		{
			Ensure.ArgumentNotNull(service, "service");

			_bindings.RemoveAll(service);

			lock (_bindingCache)
				_bindingCache.Clear();
		}

		/// <summary>
		/// Registers the specified binding.
		/// </summary>
		/// <param name="binding">The binding to add.</param>
		public override void AddBinding(IBinding binding)
		{
			Ensure.ArgumentNotNull(binding, "binding");

			_bindings.Add(binding.Service, binding);

			lock (_bindingCache)
				_bindingCache.Clear();
		}

		/// <summary>
		/// Unregisters the specified binding.
		/// </summary>
		/// <param name="binding">The binding to remove.</param>
		public override void RemoveBinding(IBinding binding)
		{
			Ensure.ArgumentNotNull(binding, "binding");

			_bindings.Remove(binding.Service, binding);

			lock (_bindingCache)
				_bindingCache.Clear();
		}

		/// <summary>
		/// Determines whether a module with the specified name has been loaded in the kernel.
		/// </summary>
		/// <param name="name">The name of the module.</param>
		/// <returns><c>True</c> if the specified module has been loaded; otherwise, <c>false</c>.</returns>
		public bool HasModule(string name)
		{
			Ensure.ArgumentNotNullOrEmpty(name, "name");
			return _modules.ContainsKey(name);
		}

		/// <summary>
		/// Gets the modules that have been loaded into the kernel.
		/// </summary>
		/// <returns>A series of loaded modules.</returns>
		public IEnumerable<INinjectModule> GetModules()
		{
			return _modules.Values.ToArray();
		}

		/// <summary>
		/// Loads the module(s) into the kernel.
		/// </summary>
		/// <param name="modules">The modules to load.</param>
		public void Load(IEnumerable<INinjectModule> modules)
		{
			Ensure.ArgumentNotNull(modules, "modules");

			foreach (INinjectModule module in modules)
			{
				INinjectModule existingModule;

				if (_modules.TryGetValue(module.Name, out existingModule))
					throw new NotSupportedException(ExceptionFormatter.ModuleWithSameNameIsAlreadyLoaded(module, existingModule));

				module.OnLoad(this);

				_modules.Add(module.Name, module);
			}
		}

		#if !NO_ASSEMBLY_SCANNING
		/// <summary>
		/// Loads modules from the files that match the specified pattern(s).
		/// </summary>
		/// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
		public void Load(IEnumerable<string> filePatterns)
		{
			var moduleLoader = Components.Get<IModuleLoader>();
			moduleLoader.LoadModules(filePatterns);
		}

		/// <summary>
		/// Loads modules defined in the specified assemblies.
		/// </summary>
		/// <param name="assemblies">The assemblies to search.</param>
		public void Load(IEnumerable<Assembly> assemblies)
		{
			foreach (Assembly assembly in assemblies)
				Load(assembly.GetNinjectModules());
		}
		#endif //!NO_ASSEMBLY_SCANNING

		/// <summary>
		/// Unloads the plugin with the specified name.
		/// </summary>
		/// <param name="name">The plugin's name.</param>
		public void Unload(string name)
		{
			Ensure.ArgumentNotNullOrEmpty(name, "name");

			INinjectModule module;

			if (!_modules.TryGetValue(name, out module))
				throw new NotSupportedException(ExceptionFormatter.NoModuleLoadedWithTheSpecifiedName(name));

			module.OnUnload(this);

			_modules.Remove(name);
		}

		/// <summary>
		/// Injects the specified existing instance, without managing its lifecycle.
		/// </summary>
		/// <param name="instance">The instance to inject.</param>
		/// <param name="parameters">The parameters to pass to the request.</param>
		public virtual void Inject(object instance, params IParameter[] parameters)
		{
			Ensure.ArgumentNotNull(instance, "instance");
			Ensure.ArgumentNotNull(parameters, "parameters");

			Type service = instance.GetType();

			var planner = Components.Get<IPlanner>();
			var pipeline = Components.Get<IPipeline>();

			var binding = new Binding(service);
			var request = CreateRequest(service, null, parameters, false, false);
			var context = CreateContext(request, binding);

			context.Plan = planner.GetPlan(service);

			var reference = new InstanceReference { Instance = instance };
			pipeline.Activate(context, reference);
		}

		/// <summary>
		/// Deactivates and releases the specified instance if it is currently managed by Ninject.
		/// </summary>
		/// <param name="instance">The instance to release.</param>
		/// <returns><see langword="True"/> if the instance was found and released; otherwise <see langword="false"/>.</returns>
		public virtual bool Release(object instance)
		{
			Ensure.ArgumentNotNull(instance, "instance");
			var cache = Components.Get<ICache>();
			return cache.Release(instance);
		}

		/// <summary>
		/// Determines whether the specified request can be resolved.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
		public virtual bool CanResolve(IRequest request)
		{
			Ensure.ArgumentNotNull(request, "request");
			var resolvers = Components.GetAll<IBindingResolver>();
			return resolvers
				.SelectMany(r => r.Resolve(_bindings, request.Service))
				.Any(SatifiesRequest(request));
		}

		/// <summary>
		/// Resolves instances for the specified request. The instances are not actually resolved
		/// until a consumer iterates over the enumerator.
		/// </summary>
		/// <param name="request">The request to resolve.</param>
		/// <returns>An enumerator of instances that match the request.</returns>
		public virtual IEnumerable<object> Resolve(IRequest request)
		{
			Ensure.ArgumentNotNull(request, "request");

			if (request.Service == typeof(IKernel))
				return new[] { this };

			if (!CanResolve(request) && !HandleMissingBinding(request.Service))
			{
				if (request.IsOptional)
					return Enumerable.Empty<object>();
				throw new ActivationException(ExceptionFormatter.CouldNotResolveBinding(request));
			}

			var bindings = GetBindings(request.Service)
				.OrderBy(binding => binding.IsConditional ? 0 : 1)
				.Where(SatifiesRequest(request));

			if (request.IsUnique && bindings.Count() > 1)
			{
				var conditionalBindings = bindings.Where(binding => binding.IsConditional);
				if (conditionalBindings.Any())
				{
					if (conditionalBindings.Count() > 1)
					{
						throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveConditionalBinding(request));
					}
					bindings = conditionalBindings;
				}
				else
				{
					var nonImplicitBindings = bindings.Where(binding => !binding.IsImplicit);
					if (nonImplicitBindings.Count() != 1)
					{
						throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveBinding(request));
					}
					bindings = nonImplicitBindings;
				}
			}
			
			return bindings
				.Select(binding => CreateContext(request, binding))
				.Select(context => context.Resolve());
		}

		/// <summary>
		/// Returns a predicate that can determine if a given IBinding matches the request.
		/// </summary>
		/// <param name="request">The request/</param>
		/// <returns>A predicate that can determine if a given IBinding matches the request.</returns>
		protected virtual Func<IBinding, bool> SatifiesRequest(IRequest request)
		{
			return binding => binding.Matches(request) && request.Matches(binding);
		}

		/// <summary>
		/// Creates a request for the specified service.
		/// </summary>
		/// <param name="service">The service that is being requested.</param>
		/// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
		/// <param name="parameters">The parameters to pass to the resolution.</param>
		/// <param name="isOptional"><c>True</c> if the request is optional; otherwise, <c>false</c>.</param>
		/// <param name="isUnique"><c>True</c> if the request should return a unique result; otherwise, <c>false</c>.</param>
		/// <returns>The created request.</returns>
		public virtual IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
		{
			Ensure.ArgumentNotNull(service, "service");
			Ensure.ArgumentNotNull(parameters, "parameters");

			return new Request(service, constraint, parameters, null, isOptional, isUnique);
		}

		/// <summary>
		/// Gets the bindings registered for the specified service.
		/// </summary>
		/// <param name="service">The service in question.</param>
		/// <returns>A series of bindings that are registered for the service.</returns>
		public virtual IEnumerable<IBinding> GetBindings(Type service)
		{
			Ensure.ArgumentNotNull(service, "service");

			lock (_bindingCache)
			{
				if (!_bindingCache.ContainsKey(service))
				{
					var resolvers = Components.GetAll<IBindingResolver>();

					resolvers
						.SelectMany(resolver => resolver.Resolve(_bindings, service))
						.Map(binding => _bindingCache.Add(service, binding));
				}

				return _bindingCache[service];
			}
		}

		/// <summary>
		/// Begins a new activation block, which can be used to deterministically dispose resolved instances.
		/// </summary>
		/// <returns>The new activation block.</returns>
		public virtual IActivationBlock BeginBlock()
		{
			return new ActivationBlock(this);
		}

		/// <summary>
		/// Creates a new builder for the specified binding.
		/// </summary>
		/// <typeparam name="T">The type restriction to apply to the binding builder.</typeparam>
		/// <param name="binding">The binding that will be built.</param>
		/// <returns>The created builder.</returns>
		protected override BindingBuilder<T> CreateBindingBuilder<T>(IBinding binding)
		{
			return new BindingBuilder<T>(binding, this);
		}

		/// <summary>
		/// Adds components to the kernel during startup.
		/// </summary>
		protected abstract void AddComponents();

		/// <summary>
		/// Attempts to handle a missing binding for a service.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns><c>True</c> if the missing binding can be handled; otherwise <c>false</c>.</returns>
		protected virtual bool HandleMissingBinding(Type service)
		{
			Ensure.ArgumentNotNull(service, "service");

			if (!TypeIsSelfBindable(service))
				return false;

			var binding = new Binding(service)
			{
				ProviderCallback = StandardProvider.GetCreationCallback(service),
				IsImplicit = true
			};

			AddBinding(binding);

			return true;
		}

		/// <summary>
		/// Returns a value indicating whether the specified service is self-bindable.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns><see langword="True"/> if the type is self-bindable; otherwise <see langword="false"/>.</returns>
		protected virtual bool TypeIsSelfBindable(Type service)
		{
			return !service.IsInterface
				&& !service.IsAbstract
				&& !service.IsValueType
				&& service != typeof(string)
				&& !service.ContainsGenericParameters;
		}

		/// <summary>
		/// Creates a context for the specified request and binding.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <param name="binding">The binding.</param>
		/// <returns>The created context.</returns>
		protected virtual IContext CreateContext(IRequest request, IBinding binding)
		{
			Ensure.ArgumentNotNull(request, "request");
			Ensure.ArgumentNotNull(binding, "binding");

			return new Context(this, request, binding, Components.Get<ICache>(), Components.Get<IPlanner>(), Components.Get<IPipeline>());
		}

		object IServiceProvider.GetService(Type service)
		{
			return this.Get(service);
		}
	}
}