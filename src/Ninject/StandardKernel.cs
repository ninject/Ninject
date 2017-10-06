// -------------------------------------------------------------------------------------------------
// <copyright file="StandardKernel.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Injection;
    using Ninject.Modules;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Planning.Bindings.Resolvers;
    using Ninject.Planning.Strategies;
    using Ninject.Selection;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// The standard implementation of a kernel.
    /// </summary>
    public class StandardKernel : KernelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="modules">The modules to load into the kernel.</param>
        public StandardKernel(params INinjectModule[] modules)
            : base(modules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        public StandardKernel(INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
        }

        /// <summary>
        /// Gets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        protected override IKernel KernelInstance
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Adds components to the kernel during startup.
        /// </summary>
        protected override void AddComponents()
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