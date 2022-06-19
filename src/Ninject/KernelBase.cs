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
        private readonly object missingBindingCacheLock = new object();

        private readonly Dictionary<string, INinjectModule> modules = new Dictionary<string, INinjectModule>();

        private readonly IBindingPrecedenceComparer bindingPrecedenceComparer;

        private Dictionary<Type, ICollection<IBinding>> bindings = new Dictionary<Type, ICollection<IBinding>>();

        private Dictionary<Type, IBinding[]> bindingCache = new Dictionary<Type, IBinding[]>();

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

            foreach (var binding in this.GetBindingsCore(request.Service))
            {
                if (this.SatifiesRequest(request, binding))
                {
                    return true;
                }
            }

            return false;
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

            foreach (var binding in this.GetBindings(request.Service))
            {
                if ((!ignoreImplicitBindings || !binding.IsImplicit) && this.SatifiesRequest(request, binding))
                {
                    return true;
                }
            }

            return false;
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
            Ensure.ArgumentNotNull(request, nameof(request));

            return this.ResolveAll(request, true);
        }

        /// <summary>
        /// Resolves an instance for the specified request.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>
        /// An instance that matches the request, or <see langword="null"/> if <see cref="IRequest.IsUnique"/> is
        /// <see langword="false"/> and there is no matching binding.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ActivationException">More than one matching bindings is available for the request, and <see cref="IRequest.IsUnique"/> is <see langword="true"/>.</exception>
        public object ResolveSingle(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            return this.ResolveSingle(request, true);
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

            return this.GetBindingsCore(service);
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
        /// Returns a value incating whether a given <see cref="IBinding"/> matches the request.
        /// </summary>
        /// <param name="request">The request/.</param>
        /// <param name="binding">The <see cref="IBinding"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="IBinding"/> matches the request; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool SatifiesRequest(IRequest request, IBinding binding)
        {
            return binding.Matches(request) && request.Matches(binding);
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

            var bindings = this.GetBindingsFromFirstResolverThatReturnsAtLeastOneBinding(request);
            if (bindings == null)
            {
                return false;
            }

            bindings.ForEach(binding => binding.IsImplicit = true);

            lock (this.missingBindingCacheLock)
            {
                if (this.CanResolve(request))
                {
                    return true;
                }

                var newBindings = new Dictionary<Type, ICollection<IBinding>>(this.bindings, new ReferenceEqualityTypeComparer());
                bindings.GroupBy(b => b.Service, b => b, (service, b) => new { service, bindings = b })
                                    .Map(
                serviceGroup =>
                {
                    if (newBindings.TryGetValue(serviceGroup.service, out ICollection<IBinding> existingBindings))
                    {
                        var newBindingList = new List<IBinding>(existingBindings);
                        newBindingList.AddRange(serviceGroup.bindings);
                        newBindings[serviceGroup.service] = newBindingList;
                    }
                    else
                    {
                        newBindings[serviceGroup.service] = new List<IBinding>(serviceGroup.bindings);
                    }
                });
                this.bindings = newBindings;
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

        private IEnumerable<object> ResolveAllWithMissingBindings(IRequest request, bool handleMissingBindings)
        {
            if (handleMissingBindings && this.HandleMissingBinding(request))
            {
                return this.ResolveAll(request, true);
            }

            if (request.IsOptional)
            {
                return Enumerable.Empty<object>();
            }

            var exceptionFormatter = this.Components.Get<IExceptionFormatter>();

            throw new ActivationException(exceptionFormatter.CouldNotResolveBinding(request));
        }

        /// <summary>
        /// Resolves an instance for the specified request.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <param name="handleMissingBindings"><see langword="true"/> to attempt to handle a missing binding request; otherwise, <see langword="false"/>.</param>
        /// <returns>
        /// An instance that matches the request, or <see langword="null"/> if <see cref="IRequest.IsUnique"/> is
        /// <see langword="false"/> and there is no matching binding.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <see langword="null"/>.</exception>
        /// <exception cref="ActivationException">More than one matching bindings is available for the request, and <see cref="IRequest.IsUnique"/> is <see langword="true"/>.</exception>
        private object ResolveSingle(IRequest request, bool handleMissingBindings)
        {
            if (request.IsUnique)
            {
                return this.ResolveSingleUnique(request, handleMissingBindings);
            }

            return this.ResolveSingleNonUnique(request, handleMissingBindings);
        }

        private IEnumerable<object> ResolveAll(IRequest request, bool handleMissingBindings)
        {
            if (request.IsUnique)
            {
                var instance = this.ResolveSingleUnique(request, handleMissingBindings);
                if (instance != null)
                {
                    yield return instance;
                }
            }
            else
            {
                foreach (var instance in this.ResolveAllNonUnique(request, handleMissingBindings))
                {
                    yield return instance;
                }
            }
        }

        private object ResolveSingleUnique(IRequest request, bool handleMissingBindings)
        {
            var bindings = this.GetBindingsCore(request.Service);
            IBinding satisfiedBinding = null;

            for (var i = 0; i < bindings.Length; i++)
            {
                var binding = bindings[i];

                if (!this.SatifiesRequest(request, binding))
                {
                    continue;
                }

                if (satisfiedBinding != null)
                {
                    if (this.bindingPrecedenceComparer.Compare(satisfiedBinding, binding) == 0)
                    {
                        if (request.IsOptional && !request.ForceUnique)
                        {
                            return null;
                        }

                        var formattedBindings = new List<string>
                            {
                                satisfiedBinding.Format(this.CreateContext(request, satisfiedBinding)),
                                binding.Format(this.CreateContext(request, binding)),
                            };

                        for (i++; i < bindings.Length; i++)
                        {
                            if (!this.SatifiesRequest(request, bindings[i]))
                            {
                                continue;
                            }

                            formattedBindings.Add(bindings[i].Format(this.CreateContext(request, bindings[i])));
                        }

                        throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveBinding(
                            request,
                            formattedBindings.ToArray()));
                    }
                    else
                    {
                        break;
                    }
                }

                satisfiedBinding = binding;
            }

            if (satisfiedBinding != null)
            {
                return this.CreateContext(request, satisfiedBinding).Resolve();
            }

            var collection = this.ResolveCollection(request);
            if (collection != null)
            {
                return collection;
            }

            if (handleMissingBindings && this.HandleMissingBinding(request))
            {
                return this.ResolveSingle(request, false);
            }

            if (request.IsOptional)
            {
                return null;
            }

            var exceptionFormatter = this.Components.Get<IExceptionFormatter>();
            throw new ActivationException(exceptionFormatter.CouldNotResolveBinding(request));
        }

        private object ResolveCollection(IRequest request)
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

                return this.ResolveAll(request, false).CastSlow(service).ToArraySlow(service);
            }

            if (request.Service.IsGenericType && !request.Service.IsGenericTypeDefinition)
            {
                var gtd = request.Service.GetGenericTypeDefinition();

                if (gtd == typeof(List<>) || gtd == typeof(IList<>) || gtd == typeof(ICollection<>))
                {
                    var service = request.Service.GenericTypeArguments[0];

                    UpdateRequest(service);

                    return this.ResolveAll(request, false).CastSlow(service).ToListSlow(service);
                }

                if (gtd == typeof(IEnumerable<>))
                {
                    var service = request.Service.GenericTypeArguments[0];

                    UpdateRequest(service);

                    return this.ResolveAll(request, false).CastSlow(service);
                }
            }

            return null;
        }

        private IEnumerable<object> ResolveAllNonUnique(IRequest request, bool handleMissingBindings)
        {
            var satisfiedBindings = this.GetBindingsCore(request.Service)
                                        .Where(b => this.SatifiesRequest(request, b));

            using (var satisfiedBindingEnumerator = satisfiedBindings.GetEnumerator())
            {
                if (!satisfiedBindingEnumerator.MoveNext())
                {
                    foreach (var instance in this.ResolveAllWithMissingBindings(request, handleMissingBindings))
                    {
                        yield return instance;
                    }
                }
            }

            if (!handleMissingBindings || satisfiedBindings.Any(binding => !binding.IsImplicit))
            {
                satisfiedBindings = satisfiedBindings.Where(binding => !binding.IsImplicit);
            }

            foreach (var satisfiedBinding in satisfiedBindings)
            {
                yield return this.CreateContext(request, satisfiedBinding).Resolve();
            }
        }

        private object ResolveSingleNonUnique(IRequest request, bool handleMissingBindings)
        {
            var bindings = this.GetBindingsCore(request.Service);
            IBinding satisfiedBinding = null;

            foreach (var binding in bindings)
            {
                if (!this.SatifiesRequest(request, binding))
                {
                    continue;
                }

                // Always prefer the first non-implicit binding that satisfies the request.
                if (!binding.IsImplicit)
                {
                    satisfiedBinding = binding;
                    break;
                }

                satisfiedBinding = binding;
            }

            if (satisfiedBinding != null)
            {
                return this.CreateContext(request, satisfiedBinding).Resolve();
            }

            var collection = this.ResolveCollection(request);
            if (collection != null)
            {
                return collection;
            }

            /*
            / We end up here if there are no bindings for the service, or if there's no binding that satisfies
            / the request.
            */

            if (handleMissingBindings && this.HandleMissingBinding(request))
            {
                return this.ResolveSingle(request, false);
            }

            if (request.IsOptional)
            {
                return null;
            }

            var exceptionFormatter = this.Components.Get<IExceptionFormatter>();
            throw new ActivationException(exceptionFormatter.CouldNotResolveBinding(request));
        }

        private List<IBinding> GetBindingsFromFirstResolverThatReturnsAtLeastOneBinding(IRequest request)
        {
            var missingBindingResolvers = this.Components.GetAll<IMissingBindingResolver>();
            return missingBindingResolvers
                .Select(c => c.Resolve(this.bindings, request).ToList())
                .FirstOrDefault(b => b.Any());
        }

        private IBinding[] GetBindingsCore(Type service)
        {
            if (this.bindingCache.TryGetValue(service, out IBinding[] result))
            {
                return result;
            }

            result = this.CreateBindings(service);
            if (result.Length > 0)
            {
                var newBindingCache = new Dictionary<Type, IBinding[]>(this.bindingCache, new ReferenceEqualityTypeComparer());
                newBindingCache[service] = result;
                this.bindingCache = newBindingCache;
            }

            return result;
        }

        private IBinding[] CreateBindings(Type service)
        {
            var bindingResolvers = this.Components.GetAll<IBindingResolver>();
            var bindingList = new List<IBinding>();
            foreach (var bindingResolver in bindingResolvers)
            {
                var bindings = bindingResolver.Resolve(this.bindings, service);
                if (bindings.Count > 0)
                {
                    bindingList.AddRange(bindings);
                }
            }

            var bindingCount = bindingList.Count;
            if (bindingCount == 0)
            {
                return Array.Empty<IBinding>();
            }
            else if (bindingCount == 1)
            {
                return new[] { bindingList[0] };
            }
            else
            {
                var bindingArray = bindingList.ToArray();
                Array.Sort(bindingArray, new ReverseComparer<IBinding>(this.bindingPrecedenceComparer));
                return bindingArray;
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