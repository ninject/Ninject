// -------------------------------------------------------------------------------------------------
// <copyright file="KernelConfiguration.cs" company="Ninject Project Contributors">
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
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Infrastructure.Language;
    using Ninject.Injection;
    using Ninject.Modules;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;
    using Ninject.Syntax;

    /// <summary>
    /// The kernel configuration.
    /// </summary>
    public class KernelConfiguration : BindingRoot, IKernelConfiguration
    {
        private readonly IDictionary<Type, ICollection<IBinding>> bindings = new Dictionary<Type, ICollection<IBinding>>();

        private readonly Dictionary<string, INinjectModule> modules = new Dictionary<string, INinjectModule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelConfiguration"/> class.
        /// </summary>
        /// <param name="modules">The modules to load into the kernel.</param>
        public KernelConfiguration(params INinjectModule[] modules)
            : this(new ComponentContainer(), new NinjectSettings(), modules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelConfiguration"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        public KernelConfiguration(INinjectSettings settings, params INinjectModule[] modules)
            : this(new ComponentContainer(), settings, modules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KernelConfiguration"/> class.
        /// </summary>
        /// <param name="components">The component container to use.</param>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        public KernelConfiguration(IComponentContainer components, INinjectSettings settings, params INinjectModule[] modules)
        {
            Ensure.ArgumentNotNull(components, "components");
            Ensure.ArgumentNotNull(settings, "settings");
            Ensure.ArgumentNotNull(modules, "modules");

            this.Settings = settings;

            this.Components = components;

            components.KernelConfiguration = this;

            this.AddComponents();

            if (this.Settings.LoadExtensions)
            {
                this.Load(this.Settings.ExtensionSearchPatterns);
            }

            this.Load(modules);
        }

        /// <summary>
        /// Gets the component container, which holds components that contribute to Ninject.
        /// </summary>
        public IComponentContainer Components { get; private set; }

        /// <summary>
        /// Gets the kernel settings.
        /// </summary>
        public override INinjectSettings Settings { get; }

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        public override void Unbind(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");

            this.bindings.Remove(service);
        }

        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public override void AddBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.bindings.Add(binding.Service, binding);
        }

        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        public override void RemoveBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.bindings.Remove(binding.Service, binding);
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
        /// Loads the module(s) into the kernel.
        /// </summary>
        /// <param name="modules">The modules to load.</param>
        public void Load(IEnumerable<INinjectModule> modules)
        {
            Ensure.ArgumentNotNull(modules, "modules");

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
        public void Load(IEnumerable<string> filePatterns)
        {
            Ensure.ArgumentNotNull(filePatterns, "filePatterns");

            var moduleLoader = this.Components.Get<IModuleLoader>();
            moduleLoader.LoadModules(filePatterns);
        }

        /// <summary>
        /// Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        public void Load(IEnumerable<Assembly> assemblies)
        {
            Ensure.ArgumentNotNull(assemblies, "assemblies");

            this.Load(assemblies.SelectMany(asm => asm.GetNinjectModules()));
        }

        /// <summary>
        /// Unloads the plugin with the specified name.
        /// </summary>
        /// <param name="name">The plugin's name.</param>
        public void Unload(string name)
        {
            Ensure.ArgumentNotNullOrEmpty(name, "name");

            if (!this.modules.TryGetValue(name, out INinjectModule module))
            {
                throw new NotSupportedException(ExceptionFormatter.NoModuleLoadedWithTheSpecifiedName(name));
            }

            module.OnUnload();

            this.modules.Remove(name);
        }

        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>A series of bindings that are registered for the service.</returns>
        public IEnumerable<IBinding> GetBindings(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");

            var resolvers = this.Components.GetAll<IBindingResolver>();

            return resolvers.SelectMany(resolver => resolver.Resolve(
                this.bindings.Keys.ToDictionary(type => type, type => this.bindings[type]),
                service));
        }

        /// <summary>
        /// Creates the readonly kernel.
        /// </summary>
        /// <returns>The readonly kernel.</returns>
        public IReadOnlyKernel BuildReadOnlyKernel()
        {
            var readonlyKernel = new ReadOnlyKernel(
                this.Settings,
                this.bindings.Clone(),
                this.Components.Get<ICache>(),
                this.Components.Get<IPlanner>(),
                this.Components.Get<IConstructorScorer>(),
                this.Components.Get<IPipeline>(),
                this.Components.Get<IBindingPrecedenceComparer>(),
                this.Components.GetAll<IBindingResolver>().ToList(),
                this.Components.GetAll<IMissingBindingResolver>().ToList());

            return readonlyKernel;
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><c>True</c> if called manually, otherwise by GC.</param>
        public override void Dispose(bool disposing)
        {
            if (!this.IsDisposed && disposing)
            {
                this.Components.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Adds components to the kernel during startup.
        /// </summary>
        protected virtual void AddComponents()
        {
            this.Components.Add<IPlanner, Planner>();
            this.Components.Add<IPlanningStrategy, ConstructorReflectionStrategy>();
            this.Components.Add<IPlanningStrategy, PropertyReflectionStrategy>();
            this.Components.Add<IPlanningStrategy, MethodReflectionStrategy>();

            this.Components.Add<ISelector, Selector>();
            this.Components.Add<IConstructorScorer, StandardConstructorScorer>();
            this.Components.Add<IInjectionHeuristic, StandardInjectionHeuristic>();

            this.Components.Add<IPipeline, Pipeline>();

            if (!this.Settings.ActivationCacheDisabled)
            {
                this.Components.Add<IActivationStrategy, ActivationCacheStrategy>();
            }

            this.Components.Add<IActivationStrategy, PropertyInjectionStrategy>();
            this.Components.Add<IActivationStrategy, MethodInjectionStrategy>();
            this.Components.Add<IActivationStrategy, InitializableStrategy>();
            this.Components.Add<IActivationStrategy, StartableStrategy>();
            this.Components.Add<IActivationStrategy, BindingActionStrategy>();
            this.Components.Add<IActivationStrategy, DisposableStrategy>();

            this.Components.Add<IBindingPrecedenceComparer, BindingPrecedenceComparer>();

            this.Components.Add<IBindingResolver, StandardBindingResolver>();
            this.Components.Add<IBindingResolver, OpenGenericBindingResolver>();

            this.Components.Add<IMissingBindingResolver, DefaultValueBindingResolver>();
            this.Components.Add<IMissingBindingResolver, SelfBindingResolver>();

#if !NO_LCG
            if (!this.Settings.UseReflectionBasedInjection)
            {
                this.Components.Add<IInjectorFactory, DynamicMethodInjectorFactory>();
            }
            else
#endif
            {
                this.Components.Add<IInjectorFactory, ReflectionInjectorFactory>();
            }

            this.Components.Add<ICache, Cache>();
            this.Components.Add<IActivationCache, ActivationCache>();
            this.Components.Add<ICachePruner, GarbageCollectionCachePruner>();

            this.Components.Add<IModuleLoader, ModuleLoader>();
            this.Components.Add<IModuleLoaderPlugin, CompiledModuleLoaderPlugin>();
            this.Components.Add<IAssemblyNameRetriever, AssemblyNameRetriever>();
        }
    }
}