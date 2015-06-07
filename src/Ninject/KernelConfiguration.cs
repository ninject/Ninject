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
            Ensure.ArgumentNotNull(components, "components");
            Ensure.ArgumentNotNull(settings, "settings");
            Ensure.ArgumentNotNull(modules, "modules");

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
            Ensure.ArgumentNotNull(service, "service");

            this.bindings.RemoveAll(service);
        }

        /// <inheritdoc />
        public override void AddBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.bindings.Add(binding.Service, binding);
        }

        /// <inheritdoc />
        public override void RemoveBinding(IBinding binding)
        {
            Ensure.ArgumentNotNull(binding, "binding");

            this.bindings.Remove(binding.Service, binding);
        }

        /// <inheritdoc />
        public IComponentContainer Components { get; private set; }

        /// <inheritdoc />
        public IEnumerable<INinjectModule> GetModules()
        {
            return this.modules.Values.ToArray();
        }

        /// <inheritdoc />
        public bool HasModule(string name)
        {
            Ensure.ArgumentNotNullOrEmpty(name, "name");
            return this.modules.ContainsKey(name);
        }

        /// <inheritdoc />
        public void Load(IEnumerable<INinjectModule> m)
        {
            Ensure.ArgumentNotNull(m, "modules");

            m = m.ToList();
            foreach (INinjectModule module in m)
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

            foreach (INinjectModule module in m)
            {
                module.OnVerifyRequiredModules();
            }
        }

#if !NO_ASSEMBLY_SCANNING
        /// <inheritdoc />
        public void Load(IEnumerable<string> filePatterns)
        {
#if PCL
            throw new NotImplementedException("Platform assembly must be referenced by app");
#else
            var moduleLoader = this.Components.Get<IModuleLoader>();
            moduleLoader.LoadModules(filePatterns);
#endif
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
            Ensure.ArgumentNotNullOrEmpty(name, "name");

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
            Ensure.ArgumentNotNull(service, "service");
            var resolvers = this.Components.GetAll<IBindingResolver>();

            return resolvers.SelectMany(resolver => resolver.Resolve(
                bindings.Keys.ToDictionary(type => type, type => bindings[type]), 
                service));
        }

        /// <inheritdoc />
        public IReadonlyKernel BuildReadonlyKernel()
        {
            var readonlyKernel = new ReadonlyKernel(
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

        private Multimap<Type, IBinding> CloneBindings()
        {
            // Todo: Clone
            return this.bindings;
        }

        /// <summary>
        /// Adds components to the kernel during startup.
        /// </summary>
        protected virtual void AddComponents()
        {
            Components.Add<IPlanner, Planner>();
            Components.Add<IPlanningStrategy, ConstructorReflectionStrategy>();
            Components.Add<IPlanningStrategy, PropertyReflectionStrategy>();
            Components.Add<IPlanningStrategy, MethodReflectionStrategy>();

            Components.Add<ISelector, Selector>();
            Components.Add<IConstructorScorer, StandardConstructorScorer>();
            Components.Add<IInjectionHeuristic, StandardInjectionHeuristic>();

            Components.Add<IPipeline, Pipeline>();
            if (!Settings.ActivationCacheDisabled)
            {
                Components.Add<IActivationStrategy, ActivationCacheStrategy>();
            }

            Components.Add<IActivationStrategy, PropertyInjectionStrategy>();
            Components.Add<IActivationStrategy, MethodInjectionStrategy>();
            Components.Add<IActivationStrategy, InitializableStrategy>();
            Components.Add<IActivationStrategy, StartableStrategy>();
            Components.Add<IActivationStrategy, BindingActionStrategy>();
            Components.Add<IActivationStrategy, DisposableStrategy>();

            Components.Add<IBindingPrecedenceComparer, BindingPrecedenceComparer>();

            Components.Add<IBindingResolver, StandardBindingResolver>();
            Components.Add<IBindingResolver, OpenGenericBindingResolver>();

            Components.Add<IMissingBindingResolver, DefaultValueBindingResolver>();
            Components.Add<IMissingBindingResolver, SelfBindingResolver>();

#if !NO_LCG
            if (!Settings.UseReflectionBasedInjection)
            {
                Components.Add<IInjectorFactory, DynamicMethodInjectorFactory>();
            }
            else
#endif
            {
                Components.Add<IInjectorFactory, ReflectionInjectorFactory>();
            }

            Components.Add<ICache, Cache>();
            Components.Add<IActivationCache, ActivationCache>();
            Components.Add<ICachePruner, GarbageCollectionCachePruner>();

#if !NO_ASSEMBLY_SCANNING
            Components.Add<IModuleLoader, ModuleLoader>();
            Components.Add<IModuleLoaderPlugin, CompiledModuleLoaderPlugin>();
            Components.Add<IAssemblyNameRetriever, AssemblyNameRetriever>();
#endif
        }
    }
}