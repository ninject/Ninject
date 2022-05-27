// -------------------------------------------------------------------------------------------------
// <copyright file="KernelBase.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

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
        private readonly object handleMissingBindingLockObject = new object();

        private readonly Dictionary<Type, ICollection<IBinding>> bindings = new Dictionary<Type, ICollection<IBinding>>();

        private readonly Dictionary<Type, IBinding[]> bindingCache = new Dictionary<Type, IBinding[]>();

        private readonly Dictionary<string, INinjectModule> modules = new Dictionary<string, INinjectModule>();

        private readonly IBindingPrecedenceComparer bindingPrecedenceComparer;

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
        /// <exception cref="ArgumentNullException"><paramref name="modules"/> is <see langword="null"/>.</exception>
        protected KernelBase(params INinjectModule[] modules)
            : this(new ComponentContainer(), new NinjectSettings(), modules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelBase"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="modules"/> is <see langword="null"/>.</exception>
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
        /// <exception cref="ArgumentNullException"><paramref name="components"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="settings"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="modules"/> is <see langword="null"/>.</exception>
        protected KernelBase(IComponentContainer components, INinjectSettings settings, params INinjectModule[] modules)
        {
            Ensure.ArgumentNotNull(components, nameof(components));
            Ensure.ArgumentNotNull(settings, nameof(settings));
            Ensure.ArgumentNotNull(modules, nameof(modules));

            this.Settings = settings;

            this.Components = components;
            components.Kernel = this;

            this.AddComponents();

            this.bindingPrecedenceComparer = this.Components.Get<IBindingPrecedenceComparer>();

            this.Bind<IKernel>().ToConstant(this).InTransientScope();
            this.Bind<IResolutionRoot>().ToConstant(this).InTransientScope();

            if (this.Settings.LoadExtensions)
            {
                this.Load(this.Settings.ExtensionSearchPatterns);
            }

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
        /// <param name="disposing"><see langword="true"/> if called manually, otherwise by GC.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
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
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        public override void Unbind(Type service)
        {
            Ensure.ArgumentNotNull(service, nameof(service));

            this.bindings.Remove(service);

            lock (this.bindingCache)
            {
                this.bindingCache.Clear();
            }
        }

        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        public override void AddBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, nameof(binding));

            this.AddBindings(new[] { binding });
        }

        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        public override void RemoveBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, nameof(binding));

            this.bindings.Remove(binding.Service, binding);

            lock (this.bindingCache)
            {
                this.bindingCache.Clear();
            }
        }

        /// <summary>
        /// Determines whether a module with the specified name has been loaded in the kernel.
        /// </summary>
        /// <param name="name">The name of the module.</param>
        /// <returns>
        /// <see langword="true"/> if the specified module has been loaded; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or a zero-length <see cref="string"/>.</exception>
        public bool HasModule(string name)
        {
            Ensure.ArgumentNotNullOrEmpty(name, nameof(name));
            return this.modules.ContainsKey(name);
        }

        /// <summary>
        /// Gets the modules that have been loaded into the kernel.
        /// </summary>
        /// <returns>
        /// A series of loaded modules.
        /// </returns>
        public IEnumerable<INinjectModule> GetModules()
        {
            return this.modules.Values.ToArray();
        }

        /// <summary>
        /// Loads the module(s) into the kernel.
        /// </summary>
        /// <param name="modules">The modules to load.</param>
        /// <exception cref="ArgumentNullException"><paramref name="modules"/> is <see langword="null"/>.</exception>
        public void Load(IEnumerable<INinjectModule> modules)
        {
            Ensure.ArgumentNotNull(modules, nameof(modules));

            modules = modules.ToList();
            foreach (INinjectModule module in modules)
            {
                if (string.IsNullOrEmpty(module.Name))
                {
                    throw new NotSupportedException(ExceptionFormatter.ModulesWithNullOrEmptyNamesAreNotSupported());
                }

                if (this.modules.TryGetValue(module.Name, out INinjectModule existingModule))
                {
                    throw new NotSupportedException(ExceptionFormatter.ModuleWithSameNameIsAlreadyLoaded(module, existingModule));
                }

                module.OnLoad(this);

                this.modules.Add(module.Name, module);
            }

            foreach (INinjectModule module in modules)
            {
                module.OnVerifyRequiredModules();
            }
        }

        /// <summary>
        /// Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePatterns"/> is <see langword="null"/>.</exception>
        public void Load(IEnumerable<string> filePatterns)
        {
            var moduleLoader = this.Components.Get<IModuleLoader>();
            moduleLoader.LoadModules(filePatterns);
        }

        /// <summary>
        /// Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assemblies"/> is <see langword="null"/>.</exception>
        public void Load(IEnumerable<Assembly> assemblies)
        {
            this.Load(assemblies.SelectMany(asm => asm.GetNinjectModules()));
        }

        /// <summary>
        /// Unloads the plugin with the specified name.
        /// </summary>
        /// <param name="name">The plugin's name.</param>
        /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or a zero-length <see cref="string"/>.</exception>
        public void Unload(string name)
        {
            Ensure.ArgumentNotNullOrEmpty(name, nameof(name));

            if (!this.modules.TryGetValue(name, out INinjectModule module))
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
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public virtual void Inject(object instance, params IParameter[] parameters)
        {
            Ensure.ArgumentNotNull(instance, nameof(instance));
            Ensure.ArgumentNotNull(parameters, nameof(parameters));

            Type service = instance.GetType();

            var planner = this.Components.Get<IPlanner>();
            var pipeline = this.Components.Get<IPipeline>();

            var binding = new Binding(service);
            var request = this.CreateRequest(service, null, parameters, false, false);
            var context = this.CreateContext(request, binding);

            context.Plan = planner.GetPlan(service);

            var reference = new InstanceReference { Instance = instance };
            pipeline.Activate(context, reference);
        }

        /// <summary>
        /// Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns>
        /// <see langword="true"/> if the instance was found and released; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is <see langword="null"/>.</exception>
        public virtual bool Release(object instance)
        {
            Ensure.ArgumentNotNull(instance, nameof(instance));
            var cache = this.Components.Get<ICache>();
            return cache.Release(instance);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// <see langword="true"/> if the request can be resolved; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public virtual bool CanResolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));
            return this.GetBindings(request.Service).Any(this.SatifiesRequest(request));
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <see langword="true"/> implicit bindings are ignored.</param>
        /// <returns>
        /// <see langword="true"/> if the request can be resolved; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        public virtual bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            Ensure.ArgumentNotNull(request, nameof(request));
            return this.GetBindings(request.Service)
                .Any(binding => (!ignoreImplicitBindings || !binding.IsImplicit) && this.SatifiesRequest(request)(binding));
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>
        /// An enumerator of instances that match the request.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ActivationException">More than one matching bindings is available for the request, and <see cref="IRequest.IsUnique"/> is <see langword="true"/>.</exception>
        public virtual IEnumerable<object> Resolve(IRequest request)
        {
            return this.Resolve(request, true, false);
        }

        /// <summary>
        /// Creates a request for the specified service.
        /// </summary>
        /// <param name="service">The service that is being requested.</param>
        /// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
        /// <param name="parameters">The parameters to pass to the resolution.</param>
        /// <param name="isOptional"><see langword="true"/> if the request is optional; otherwise, <see langword="false"/>.</param>
        /// <param name="isUnique"><see langword="true"/> if the request should return a unique result; otherwise, <see langword="false"/>.</param>
        /// <returns>
        /// The request for the specified service.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
        public virtual IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IReadOnlyList<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Ensure.ArgumentNotNull(service, nameof(service));
            Ensure.ArgumentNotNull(parameters, nameof(parameters));

            return new Request(service, constraint, parameters, null, isOptional, isUnique);
        }

        /// <summary>
        /// Begins a new activation block, which can be used to deterministically dispose resolved instances.
        /// </summary>
        /// <returns>
        /// The new activation block.
        /// </returns>
        public virtual IActivationBlock BeginBlock()
        {
            return new ActivationBlock(this);
        }

        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>
        /// A series of bindings that are registered for the service.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
        public virtual IBinding[] GetBindings(Type service)
        {
            Ensure.ArgumentNotNull(service, nameof(service));

            lock (this.bindingCache)
            {
                if (!this.bindingCache.ContainsKey(service))
                {
                    var resolvers = this.Components.GetAll<IBindingResolver>();

                    var compiledBindings = resolvers
                        .SelectMany(resolver => resolver.Resolve(this.bindings, service))
                        .OrderByDescending(b => b, this.bindingPrecedenceComparer).ToArray();
                    this.bindingCache.Add(service, compiledBindings);

                    return compiledBindings;
                }

                return this.bindingCache[service];
            }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="service">The service type.</param>
        /// <returns>The service object.</returns>
        object IServiceProvider.GetService(Type service)
        {
            return this.Settings.ThrowOnGetServiceNotFound
               ? this.Get(service)
               : this.TryGet(service);
        }

        /// <summary>
        /// Returns a predicate that can determine if a given IBinding matches the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// A predicate that can determine if a given <see cref="IBinding"/> matches the request.
        /// </returns>
        protected virtual Func<IBinding, bool> SatifiesRequest(IRequest request)
        {
            return binding => binding.Matches(request) && request.Matches(binding);
        }

        /// <summary>
        /// Adds components to the kernel during startup.
        /// </summary>
        protected abstract void AddComponents();

        /// <summary>
        /// Attempts to handle a missing binding for a request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// <see langword="true"/> if the missing binding can be handled; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool HandleMissingBinding(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            var components = this.Components.GetAll<IMissingBindingResolver>();

            // Take the first set of bindings that resolve.
            var bindings = components
                .Select(c => c.Resolve(this.bindings, request).ToList())
                .FirstOrDefault(b => b.Any());

            if (bindings == null)
            {
                return false;
            }

            lock (this.handleMissingBindingLockObject)
            {
                if (!this.CanResolve(request))
                {
                    bindings.Map(binding => binding.IsImplicit = true);
                    this.AddBindings(bindings);
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a context for the specified request and binding.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="binding">The binding.</param>
        /// <returns>
        /// The created context.
        /// </returns>
        protected virtual IContext CreateContext(IRequest request, IBinding binding)
        {
            Ensure.ArgumentNotNull(request, nameof(request));
            Ensure.ArgumentNotNull(binding, nameof(binding));

            return new Context(this, request, binding, this.Components.Get<ICache>(), this.Components.Get<IPlanner>(), this.Components.Get<IPipeline>(), this.Components.Get<IExceptionFormatter>());
        }

        private IEnumerable<object> Resolve(IRequest request, bool handleMissingBindings, bool filterImplicitBindings)
        {
            void UpdateRequest(Type service)
            {
                if (request.ParentRequest == null)
                {
                    request = this.CreateRequest(service, null, request.Parameters.GetShouldInheritParameters(), true, false);
                }
                else
                {
                    request = request.ParentRequest.CreateChild(service, request.ParentContext, request.Target);
                    request.IsOptional = true;
                }
            }

            if (request.Service.IsArray)
            {
                var service = request.Service.GetElementType();

                UpdateRequest(service);

                return new[] { this.Resolve(request, false, true).CastSlow(service).ToArraySlow(service) };
            }

            if (request.Service.IsGenericType)
            {
                var gtd = request.Service.GetGenericTypeDefinition();

                if (gtd == typeof(List<>) || gtd == typeof(IList<>) || gtd == typeof(ICollection<>))
                {
                    var service = request.Service.GenericTypeArguments[0];

                    UpdateRequest(service);

                    return new[] { this.Resolve(request, false, true).CastSlow(service).ToListSlow(service) };
                }

                if (gtd == typeof(IEnumerable<>))
                {
                    var service = request.Service.GenericTypeArguments[0];

                    UpdateRequest(service);

                    return new[] { this.Resolve(request, false, true).CastSlow(service) };
                }
            }

            var satisfiedBindings = this.GetBindings(request.Service)
                                        .Where(this.SatifiesRequest(request));

            if (filterImplicitBindings)
            {
                satisfiedBindings = satisfiedBindings.Where(binding => binding.IsImplicit == false);
            }

            var satisfiedBindingEnumerator = satisfiedBindings.GetEnumerator();

            if (!satisfiedBindingEnumerator.MoveNext())
            {
                if (handleMissingBindings && this.HandleMissingBinding(request))
                {
                    return this.Resolve(request, false, false);
                }

                if (request.IsOptional)
                {
                    return Enumerable.Empty<object>();
                }

                var exceptionFormatter = this.Components.Get<IExceptionFormatter>();

                throw new ActivationException(exceptionFormatter.CouldNotResolveBinding(request));
            }

            if (request.IsUnique)
            {
                var selectedBinding = satisfiedBindingEnumerator.Current;

                if (satisfiedBindingEnumerator.MoveNext() &&
                    this.bindingPrecedenceComparer.Compare(selectedBinding, satisfiedBindingEnumerator.Current) == 0)
                {
                    if (request.IsOptional && !request.ForceUnique)
                    {
                        return Enumerable.Empty<object>();
                    }

                    var formattedBindings =
                        from binding in satisfiedBindings
                        let context = this.CreateContext(request, binding)
                        select binding.Format(context);

                    throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveBinding(
                        request,
                        formattedBindings.ToArray()));
                }

                return new[] { this.CreateContext(request, selectedBinding).Resolve() };
            }
            else
            {
                if (satisfiedBindings.Any(binding => !binding.IsImplicit))
                {
                    satisfiedBindings = satisfiedBindings.Where(binding => !binding.IsImplicit);
                }

                return satisfiedBindings
                    .Select(binding => this.CreateContext(request, binding).Resolve());
            }
        }

        private void AddBindings(IEnumerable<IBinding> bindings)
        {
            bindings.Map(binding => this.bindings.Add(binding.Service, binding));

            lock (this.bindingCache)
            {
                this.bindingCache.Clear();
            }
        }
    }
}