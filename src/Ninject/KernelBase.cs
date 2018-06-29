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

#pragma warning disable CS0618 // Type or member is obsolete
namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Ninject.Activation;
    using Ninject.Activation.Blocks;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Modules;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// The base implementation of an <see cref="IKernel"/>.
    /// </summary>
    public abstract class KernelBase : BindingRoot, IKernel
    {
        private readonly object kernelLockObject = new object();

        private readonly IKernelConfiguration kernelConfiguration;

        private IReadOnlyKernel kernel;

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
            Ensure.ArgumentNotNull(components, "components");
            Ensure.ArgumentNotNull(settings, "settings");
            Ensure.ArgumentNotNull(modules, "modules");

            this.kernelConfiguration = new KernelConfiguration(components, settings, modules);

            this.kernelConfiguration.Bind<IKernel>().ToConstant(this).InTransientScope();
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

        private IReadOnlyKernel ReadOnlyKernel
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
                        this.kernel = this.kernelConfiguration.BuildReadOnlyKernel();
                        this.isDirty = false;
                    }

                    return this.kernel;
                }
            }
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><c>True</c> if called manually, otherwise by GC.</param>
        public override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                this.kernelConfiguration.Dispose();
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
        /// <param name="modules">The modules to load.</param>
        public void Load(IEnumerable<INinjectModule> modules)
        {
            this.kernelConfiguration.Load(modules);
            this.isDirty = true;
        }

        /// <summary>
        /// Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        public void Load(IEnumerable<string> filePatterns)
        {
            this.kernelConfiguration.Load(filePatterns);
            this.isDirty = true;
        }

        /// <summary>
        /// Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        public void Load(IEnumerable<Assembly> assemblies)
        {
            this.kernelConfiguration.Load(assemblies);
            this.isDirty = true;
        }

        /// <summary>
        /// Unloads the plugin with the specified name.
        /// </summary>
        /// <param name="name">The plugin's name.</param>
        public void Unload(string name)
        {
            this.kernelConfiguration.Unload(name);
            this.isDirty = true;
        }

        /// <summary>
        /// Injects the specified existing instance, without managing its lifecycle.
        /// </summary>
        /// <param name="instance">The instance to inject.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        public virtual void Inject(object instance, params IParameter[] parameters)
        {
            this.ReadOnlyKernel.Inject(instance, parameters);
        }

        /// <summary>
        /// Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns><see langword="True"/> if the instance was found and released; otherwise <see langword="false"/>.</returns>
        public virtual bool Release(object instance)
        {
            return this.ReadOnlyKernel.Release(instance);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public virtual bool CanResolve(IRequest request)
        {
            return this.ReadOnlyKernel.CanResolve(request);
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
            return this.ReadOnlyKernel.CanResolve(request, ignoreImplicitBindings);
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>An enumerator of instances that match the request.</returns>
        public virtual IEnumerable<object> Resolve(IRequest request)
        {
            return this.ReadOnlyKernel.Resolve(request);
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
            return this.ReadOnlyKernel.CreateRequest(service, constraint, parameters, isOptional, isUnique);
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

        /// <summary>
        /// Creates the readonly kernel.
        /// </summary>
        /// <returns>The readonly kernel.</returns>
        public IReadOnlyKernel BuildReadOnlyKernel()
        {
            throw new NotSupportedException("Kernel is built internally.");
        }

        // Todo: Add
        // protected virtual IComparer<IBinding> GetBindingPrecedenceComparer()
        // {
        //    return new BindingPrecedenceComparer();
        // }

        // Todo: Add
        // protected virtual Func<IBinding, bool> SatifiesRequest(IRequest request)
        // {
        //    return binding => binding.Matches(request) && request.Matches(binding);
        // }

        // Todo: Add
        // protected abstract void AddComponents();

        // Todo: Add
        // protected virtual IContext CreateContext(IRequest request, IBinding binding)
        // {
        //    Ensure.ArgumentNotNull(request, "request");
        //    Ensure.ArgumentNotNull(binding, "binding");

        // return new Context(this, request, binding, this.Components.Get<ICache>(), this.Components.Get<IPlanner>(), this.Components.Get<IPipeline>());
        // }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service object.</returns>
        public object GetService(Type serviceType)
        {
            return this.ReadOnlyKernel.GetService(serviceType);
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete