// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

namespace Ninject
{
    using Ninject.Activation;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Injection;
    using Ninject.Modules;
    using Ninject.Planning;
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
        public StandardKernel(params INinjectModule[] modules) : base(modules)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        public StandardKernel(INinjectSettings settings, params INinjectModule[] modules) : base(settings, modules)
        {
        }

        /// <summary>
        /// Adds components to the kernel during startup.
        /// </summary>
        protected override void AddComponents()
        {
            Components.Add<IPlanner, Planner>();
            Components.Add<IPlanningStrategy, ConstructorReflectionStrategy>();
            Components.Add<IPlanningStrategy, PropertyReflectionStrategy>();
            Components.Add<IPlanningStrategy, MethodReflectionStrategy>();

            Components.Add<ISelector, Selector>();
            Components.Add<IConstructorScorer, StandardConstructorScorer>();
            Components.Add<IInjectionHeuristic, StandardInjectionHeuristic>();

            Components.Add<IPipeline, Pipeline>();
            Components.Add<IActivationStrategy, ActivationCacheStrategy>();
            Components.Add<IActivationStrategy, PropertyInjectionStrategy>();
            Components.Add<IActivationStrategy, MethodInjectionStrategy>();
            Components.Add<IActivationStrategy, InitializableStrategy>();
            Components.Add<IActivationStrategy, StartableStrategy>();
            Components.Add<IActivationStrategy, BindingActionStrategy>();
            Components.Add<IActivationStrategy, DisposableStrategy>();

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
            #endif
        }
    }
}
