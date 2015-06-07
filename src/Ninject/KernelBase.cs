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
    using System.Reflection;
    using Ninject.Activation;
    using Ninject.Activation.Blocks;
    using Ninject.Components;
    using Ninject.Modules;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Selection;
    using Ninject.Syntax;

    /// <summary>
    /// The base implementation of an <see cref="IKernel"/>.
    /// </summary>
    [Obsolete("Use ReadonlyKernelBase and KernelConfigurationBase")]
    public abstract class KernelBase : BindingRoot, IKernel
    {
        private readonly object kernelLockObject = new object();
        
        private readonly IKernelConfiguration kernelConfiguration;

        private IReadonlyKernel kernel;

        private bool isDirty = true;

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
            this.kernelConfiguration = new KernelConfiguration(components, settings, modules);
            this.kernelConfiguration.Bind<IKernel>().ToMethod(ctx => this);
            this.kernelConfiguration.Bind<IResolutionRoot>().ToMethod(ctx => this).When(ctx => true);
            }

        /// <summary>
        /// Gets the kernel settings.
        /// </summary>
        public override INinjectSettings Settings
        {
            get { return this.kernelConfiguration.Settings; }
        }

        /// <summary>
        /// Gets the component container, which holds components that contribute to Ninject.
        /// </summary>
        public IComponentContainer Components
        {
            get { return this.kernelConfiguration.Components; }
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                if (this.kernel != null)
                {
                    this.kernel.Dispose();
                }

                //this.kernelConfiguration.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        public override void Unbind(Type service)
        {
            this.kernelConfiguration.Unbind(service);
            this.isDirty = true;
        }

        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public override void AddBinding(IBinding binding)
        {
            this.kernelConfiguration.AddBinding(binding);
            this.isDirty = true;
        }

        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        public override void RemoveBinding(IBinding binding)
        {
            this.kernelConfiguration.RemoveBinding(binding);
            this.isDirty = true;
        }

        /// <summary>
        /// Determines whether a module with the specified name has been loaded in the kernel.
        /// </summary>
        /// <param name="name">The name of the module.</param>
        /// <returns><c>True</c> if the specified module has been loaded; otherwise, <c>false</c>.</returns>
        public bool HasModule(string name)
        {
            return this.kernelConfiguration.HasModule(name);
        }

        /// <summary>
        /// Gets the modules that have been loaded into the kernel.
        /// </summary>
        /// <returns>A series of loaded modules.</returns>
        public IEnumerable<INinjectModule> GetModules()
        {
            return this.kernelConfiguration.GetModules();
        }

        /// <summary>
        /// Loads the module(s) into the kernel.
        /// </summary>
        /// <param name="m">The modules to load.</param>
        public void Load(IEnumerable<INinjectModule> m)
        {
            this.kernelConfiguration.Load(m);
            this.isDirty = true;
                }
                
#if !NO_ASSEMBLY_SCANNING
        /// <summary>
        /// Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        public void Load(IEnumerable<string> filePatterns)
        {
#if PCL
            throw new NotImplementedException();
#else
            this.kernelConfiguration.Load(filePatterns);
            this.isDirty = true;
#endif
        }

#if WINRT
        /// <summary>
        /// Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        public async System.Threading.Tasks.Task LoadAsync(IEnumerable<string> filePatterns)
        {
            var moduleLoader = this.Components.Get<IModuleLoader>();
            await moduleLoader.LoadModules(filePatterns);
        }
#endif

        /// <summary>
        /// Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        public void Load(IEnumerable<Assembly> assemblies)
        {
            this.kernelConfiguration.Load(assemblies);
            this.isDirty = true;
        }
#else
        /// <summary>
        /// Does nothing on this framework
        /// </summary>
        /// <param name="filePatterns"></param>
        public void Load(IEnumerable<string> filePatterns)
        {
            
        }

        /// <summary>
        /// Does nothing on this framework
        /// </summary>
        /// <param name="assembly"></param>
        public void Load(IEnumerable<Assembly> assembly)
        {
            
        }
#endif //!NO_ASSEMBLY_SCANNING

        /// <summary>
        /// Unloads the plugin with the specified name.
        /// </summary>
        /// <param name="name">The plugin's name.</param>
        public void Unload(string name)
        {
            this.kernelConfiguration.Unload(name);
            this.isDirty = true;
        }

        private IReadonlyKernel ReadonlyKernel
        {
            get
            {
                if (!this.isDirty)
                {
                    return this.kernel;
                }

                lock (this.kernelLockObject)
            {
                    if (this.isDirty)
                    {
                        this.kernel = this.kernelConfiguration.BuildReadonlyKernel();
                        this.isDirty = false;
            }

                    return this.kernel;
        }
            }
        }

        /// <summary>
        /// Injects the specified existing instance, without managing its lifecycle.
        /// </summary>
        /// <param name="instance">The instance to inject.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        public virtual void Inject(object instance, params IParameter[] parameters)
        {
            this.ReadonlyKernel.Inject(instance, parameters);
        }

        /// <summary>
        /// Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns><see langword="True"/> if the instance was found and released; otherwise <see langword="false"/>.</returns>
        public virtual bool Release(object instance)
        {
            return this.ReadonlyKernel.Release(instance);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public virtual bool CanResolve(IRequest request)
        {
            return this.ReadonlyKernel.CanResolve(request);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <c>true</c> implicit bindings are ignored.</param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            return this.ReadonlyKernel.CanResolve(request, ignoreImplicitBindings);
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>An enumerator of instances that match the request.</returns>
        public virtual IEnumerable<object> Resolve(IRequest request)
        {
            return this.ReadonlyKernel.Resolve(request);
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
            return this.ReadonlyKernel.CreateRequest(service, constraint, parameters, isOptional, isUnique);
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
            return this.kernelConfiguration.GetBindings(service);
                }

        /// <inheritdoc />
        public IReadonlyKernel BuildReadonlyKernel()
        {
            throw new NotSupportedException("Kernel is built internally.");
        }

        // Todo: Add
        //protected virtual IComparer<IBinding> GetBindingPrecedenceComparer()
        //{
        //    return new BindingPrecedenceComparer();
        //}

        // Todo: Add
        //protected virtual Func<IBinding, bool> SatifiesRequest(IRequest request)
        //{
        //    return binding => binding.Matches(request) && request.Matches(binding);
        //}

        // Todo: Add
        //protected abstract void AddComponents();

        // Todo: Add
        //protected virtual IContext CreateContext(IRequest request, IBinding binding)
        //{
        //    Ensure.ArgumentNotNull(request, "request");
        //    Ensure.ArgumentNotNull(binding, "binding");

        //    return new Context(this, request, binding, this.Components.Get<ICache>(), this.Components.Get<IPlanner>(), this.Components.Get<IPipeline>());
        //}

        /// <inheritdoc />
        public object GetService(Type serviceType)
                            {
            return this.ReadonlyKernel.GetService(serviceType);
        }
    }
}