//-------------------------------------------------------------------------------------------------
// <copyright file="KernelConfiguration.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2016, Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
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
//-------------------------------------------------------------------------------------------------

namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
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
    /// The kernel configuration
    /// </summary>
    public class KernelConfiguration : BindingRoot, IKernelConfiguration
    {
        private readonly INinjectSettings settings;

        private readonly Multimap<Type, IBinding> bindings = new Multimap<Type, IBinding>();

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
            Contract.Requires(components != null);
            Contract.Requires(settings != null);
            Contract.Requires(modules != null);

            this.settings = settings;

            this.Components = components;

            components.KernelConfiguration = this;

            this.AddComponents();

#if !NO_ASSEMBLY_SCANNING
            if (this.settings.LoadExtensions)
            {
                this.Load(this.settings.ExtensionSearchPatterns);
            }
#endif
            this.Load(modules);
        }

        /// <inheritdoc />
        public IComponentContainer Components { get; private set; }

        /// <inheritdoc />
        public override INinjectSettings Settings
        {
            get
            {
                return this.settings;
            }
        }

        /// <inheritdoc />
        public override void Unbind(Type service)
        {
            Contract.Requires(service != null);

            this.bindings.RemoveAll(service);
        }

        /// <inheritdoc />
        public override void AddBinding(IBinding binding)
        {
            Contract.Requires(binding != null);

            this.bindings.Add(binding.Service, binding);
        }

        /// <inheritdoc />
        public override void RemoveBinding(IBinding binding)
        {
            Contract.Requires(binding != null);

            this.bindings.Remove(binding.Service, binding);
        }

        /// <inheritdoc />
        public IEnumerable<INinjectModule> GetModules()
        {
            return this.modules.Values.ToArray();
        }

        /// <inheritdoc />
        public bool HasModule(string name)
        {
            Contract.Requires(name != null);
            return this.modules.ContainsKey(name);
        }

        /// <inheritdoc />
        public void Load(IEnumerable<INinjectModule> modules)
        {
            Contract.Requires(this.modules != null);

            modules = modules.ToList();
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

            foreach (INinjectModule module in modules)
            {
                module.OnVerifyRequiredModules();
            }
        }

#if !NO_ASSEMBLY_SCANNING
        /// <inheritdoc />
        public void Load(IEnumerable<string> filePatterns)
        {
            var moduleLoader = this.Components.Get<IModuleLoader>();
            moduleLoader.LoadModules(filePatterns);
        }

        /// <inheritdoc />
        public void Load(IEnumerable<Assembly> assemblies)
        {
            this.Load(assemblies.SelectMany(asm => asm.GetNinjectModules()));
        }
#endif //!NO_ASSEMBLY_SCANNING

        /// <inheritdoc />
        public void Unload(string name)
        {
            Contract.Requires(name != null);

            INinjectModule module;

            if (!this.modules.TryGetValue(name, out module))
            {
                throw new NotSupportedException(ExceptionFormatter.NoModuleLoadedWithTheSpecifiedName(name));
            }

            module.OnUnload(this);

            this.modules.Remove(name);
        }

        /// <inheritdoc />
        public IEnumerable<IBinding> GetBindings(Type service)
        {
            Contract.Requires(service != null);
            var resolvers = this.Components.GetAll<IBindingResolver>();

            return resolvers.SelectMany(resolver => resolver.Resolve(
                this.bindings.Keys.ToDictionary(type => type, type => this.bindings[type]),
                service));
        }

        /// <inheritdoc />
        public IReadOnlyKernel BuildReadonlyKernel()
        {
            var readonlyKernel = new ReadOnlyKernel(
                this.CloneBindings(),
                this.Components.Get<ICache>(),
                this.Components.Get<IPlanner>(),
                this.Components.Get<IPipeline>(),
                this.Components.Get<IBindingPrecedenceComparer>(),
                this.Components.GetAll<IBindingResolver>().ToList(),
                this.Components.GetAll<IMissingBindingResolver>().ToList(),
                this.Settings.Clone(),
                this.Components.Get<ISelector>());

            return readonlyKernel;
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

#if !NO_ASSEMBLY_SCANNING
            this.Components.Add<IModuleLoader, ModuleLoader>();
            this.Components.Add<IModuleLoaderPlugin, CompiledModuleLoaderPlugin>();
            this.Components.Add<IAssemblyNameRetriever, AssemblyNameRetriever>();
#endif
        }

        private Multimap<Type, IBinding> CloneBindings()
        {
            // Todo: Clone
            return this.bindings;
        }
    }
}