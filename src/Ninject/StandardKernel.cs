//-------------------------------------------------------------------------------
// <copyright file="StandardKernel.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
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
//-------------------------------------------------------------------------------

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
