// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Ninject.Activation;
    using Ninject.Activation.Blocks;
    using Ninject.Activation.Caching;
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

    /// <summary>
    /// The base implementation of an <see cref="IKernel"/>.
    /// </summary>
    public abstract class KernelBase : BindingRoot, IKernel
    {
        /// <summary>
        /// Lock used when adding missing bindings.
        /// </summary>
        protected readonly object HandleMissingBindingLockObject = new object();
        
        private readonly Multimap<Type, IBinding> bindings = new Multimap<Type, IBinding>();

        private readonly Multimap<Type, IBinding> bindingCache = new Multimap<Type, IBinding>();

        private readonly Dictionary<string, INinjectModule> modules = new Dictionary<string, INinjectModule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBase"/> class.
        /// </summary>
        protected KernelBase()
            : this(new ComponentContainer(), new NinjectSettings(), new INinjectModule[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBase"/> class.
        /// </summary>
        /// <param name="modules">The modules to load into the kernel.</param>
        protected KernelBase(params INinjectModule[] modules)
            : this(new ComponentContainer(), new NinjectSettings(), modules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBase"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        protected KernelBase(INinjectSettings settings, params INinjectModule[] modules)
            : this(new ComponentContainer(), settings, modules)
        {
        }

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

            this.Settings = settings;

            this.Components = components;
            components.Kernel = this;

            this.AddComponents();

#if !NO_WEB
            OnePerRequestModule.StartManaging(this);
#endif

#if !NO_ASSEMBLY_SCANNING
            if (this.Settings.LoadExtensions)
            {
                this.Load(new[] { this.Settings.ExtensionSearchPattern });
            }
#endif

            this.Load(modules);
        }

        /// <summary>
        /// Gets the kernel settings.
        /// </summary>
        public INinjectSettings Settings { get; private set; }

        /// <summary>
        /// Gets the component container, which holds components that contribute to Ninject.
        /// </summary>
        public IComponentContainer Components { get; private set; }

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

                if (this.Components != null)
                {
                    // Deactivate all cached instances before shutting down the kernel.
                    var cache = this.Components.Get<ICache>();
                    cache.Clear();

                    this.Components.Dispose();
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

            this.bindings.RemoveAll(service);

            lock (this.bindingCache)
            {
                this.bindingCache.Clear();
            }
        }

        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public override void AddBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.AddBindings(new[] { binding });
        }

        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        public override void RemoveBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.bindings.Remove(binding.Service, binding);

            lock (this.bindingCache)
                this.bindingCache.Clear();
        }

        /// <summary>
        /// Determines whether a module with the specified name has been loaded in the kernel.
        /// </summary>
        /// <param name="name">The name of the module.</param>
        /// <returns><c>True</c> if the specified module has been loaded; otherwise, <c>false</c>.</returns>
        public bool HasModule(string name)
        {
            Ensure.ArgumentNotNullOrEmpty(name, "name");
            return this.modules.ContainsKey(name);
        }

        /// <summary>
        /// Gets the modules that have been loaded into the kernel.
        /// </summary>
        /// <returns>A series of loaded modules.</returns>
        public IEnumerable<INinjectModule> GetModules()
        {
            return this.modules.Values.ToArray();
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
                if (string.IsNullOrEmpty(module.Name))
                {
                    throw new NotSupportedException(ExceptionFormatter.ModulesWithNullOrEmptyNamesAreNotSupported());
                }
                
                INinjectModule existingModule;

                if (this.modules.TryGetValue(module.Name, out existingModule))
                {
                    throw new NotSupportedException(ExceptionFormatter.ModuleWithSameNameIsAlreadyLoaded(module, existingModule));
                }

                module.OnLoad(this);

                this.modules.Add(module.Name, module);
            }
        }

#if !NO_ASSEMBLY_SCANNING
        /// <summary>
        /// Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        public void Load(IEnumerable<string> filePatterns)
        {
            var moduleLoader = this.Components.Get<IModuleLoader>();
            moduleLoader.LoadModules(filePatterns);
        }

        /// <summary>
        /// Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        public void Load(IEnumerable<Assembly> assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                this.Load(assembly.GetNinjectModules());
            }
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

            if (!this.modules.TryGetValue(name, out module))
            {
                throw new NotSupportedException(ExceptionFormatter.NoModuleLoadedWithTheSpecifiedName(name));
            }

            module.OnUnload(this);

            this.modules.Remove(name);
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

            var planner = this.Components.Get<IPlanner>();
            var pipeline = this.Components.Get<IPipeline>();

            var binding = new Binding(service);
            var request = this.CreateRequest(service, null, parameters, false, false);
            var context = this.CreateContext(request, binding);

            lock (planner)
            {
                context.Plan = planner.GetPlan(service);
            }

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
            var cache = this.Components.Get<ICache>();
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
            return this.GetSatisifiedBindings(request).Any();
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
            {
                return new[] { this };
            }

            var bindingPrecedenceComparer = this.GetBindingPrecedenceComparer();

            var resolveBindings = this.GetSatisifiedBindings(request);

            resolveBindings = (resolveBindings.Any() ? resolveBindings : this.GetMissingBindings(request))
                              .OrderByDescending(b => b, bindingPrecedenceComparer)
                              .ToList();

            if (!resolveBindings.Any())
            {
                if (request.IsOptional)
                {
                    return Enumerable.Empty<object>();
                }

                throw new ActivationException(ExceptionFormatter.CouldNotResolveBinding(request));
            }

            var model = resolveBindings.First();
            resolveBindings = resolveBindings.TakeWhile(binding => bindingPrecedenceComparer.Compare(binding, model) == 0);

            if (request.IsUnique && resolveBindings.Count() > 1)
            {
                if (request.IsOptional)
                {
                    return Enumerable.Empty<object>();
                }

                throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveBinding(request));
            }

            return resolveBindings.Select(binding => this.CreateContext(request, binding).Resolve());
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
        /// Begins a new activation block, which can be used to deterministically dispose resolved instances.
        /// </summary>
        /// <returns>The new activation block.</returns>
        public virtual IActivationBlock BeginBlock()
        {
            return new ActivationBlock(this);
        }

        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>A series of bindings that are registered for the service.</returns>
        public virtual IEnumerable<IBinding> GetBindings(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");

            lock (this.bindingCache)
            {
                if (!this.bindingCache.ContainsKey(service))
                {
                    var resolvers = this.Components.GetAll<IBindingResolver>();

                    resolvers
                        .SelectMany(resolver => resolver.Resolve(this.bindings, service))
                        .Map(binding => this.bindingCache.Add(service, binding));
                }

                // materialize this while we're in the lock otherwise
                // the cache can change underneath us
                return this.bindingCache[service].ToList();
            }
        }

        /// <summary>
        /// Returns an IComparer that is used to determine resolution precedence.
        /// </summary>
        /// <returns>An IComparer that is used to determine resolution precedence.</returns>
        protected virtual IComparer<IBinding> GetBindingPrecedenceComparer()
        {
            return new BindingPrecedenceComparer();
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
        [Obsolete]
        protected virtual bool HandleMissingBinding(Type service)
        {
            return false;
        }

        /// <summary>
        /// Attempts to handle a missing binding for a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the missing binding can be handled; otherwise <c>false</c>.</returns>
        [Obsolete]
        protected virtual bool HandleMissingBinding(IRequest request)
        {
#pragma warning disable 612,618 // allow call to obsolete for backwards compatibility
            return this.HandleMissingBinding(request.Service);
#pragma warning restore 612,618
        }

        /// <summary>
        /// Attempts to handle a missing binding for a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A list of bindings that can satisfy the request or an empty set.</returns>
        protected virtual IEnumerable<IBinding> GetMissingBindings(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

#pragma warning disable 612,618 // allow call to obsolete for backwards compatibility
            // old-style handling added bindings directly to the map
            if (this.HandleMissingBinding(request))
            {
                return this.GetSatisifiedBindings(request);
            }
#pragma warning restore 612,618

            // Take the first set of bindings that resolve.
            var missingBindings =  this.Components.GetAll<IMissingBindingResolver>()
                                                  .Select(c => c.Resolve(this.bindings, request))
                                                  .FirstOrDefault(b => b.Any(this.SatifiesRequest(request)))
                                                  ?? Enumerable.Empty<IBinding>();

            // Any bindings listed as Implicit can be added to the bindings list.
            var implicitBindings = missingBindings.Where(b => b.IsImplicit);
            if(implicitBindings.Any())
            {
                lock(HandleMissingBindingLockObject)
                {
                    this.AddBindings(implicitBindings);
                }
            }

            return missingBindings;
        }

        /// <summary>
        /// Returns a value indicating whether the specified service is self-bindable.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns><see langword="True"/> if the type is self-bindable; otherwise <see langword="false"/>.</returns>
        [Obsolete]
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

            return new Context(this, request, binding, this.Components.Get<ICache>(), this.Components.Get<IPlanner>(), this.Components.Get<IPipeline>());
        }

        private IEnumerable<IBinding> GetSatisifiedBindings(IRequest request)
        {
            return this.GetBindings(request.Service)
                       .Where(this.SatifiesRequest(request));
        }

        private void AddBindings(IEnumerable<IBinding> bindings)
        {
            bindings.Map(binding => this.bindings.Add(binding.Service, binding));

            lock (this.bindingCache)
                this.bindingCache.Clear();
        }

        object IServiceProvider.GetService(Type service)
        {
            return this.Get(service);
        }

        private class BindingPrecedenceComparer : IComparer<IBinding>
        {
            public int Compare(IBinding x, IBinding y)
            {
                if (x == y)
                {
                    return 0;
                }

                // Each function represents a level of precedence.
                var funcs = new List<Func<IBinding, bool>>
                            {
                                b => b != null,       // null bindings should never happen, but just in case
                                b => b.IsConditional, // conditional bindings > unconditional
                                b => !b.IsImplicit,   // explicit bindings > implicit
                            };

                var q = from func in funcs
                        let xVal = func(x)
                        where xVal != func(y) 
                        select xVal ? 1 : -1;

                // returns the value of the first function that represents a difference
                // between the bindings, or else returns 0 (equal)
                return q.FirstOrDefault();
            }
        }
    }
}