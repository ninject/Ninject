// -------------------------------------------------------------------------------------------------
// <copyright file="IKernel.cs" company="Ninject Project Contributors">
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
    using System.Reflection;

    using Ninject.Activation.Blocks;
    using Ninject.Components;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Modules;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// A super-factory that can create objects of all kinds, following hints provided by <see cref="IBinding"/>s.
    /// </summary>
    public interface IKernel : IBindingRoot, IResolutionRoot, IServiceProvider, IDisposableObject, INotifyWhenDisposed
    {
        /// <summary>
        /// Gets the kernel settings.
        /// </summary>
        INinjectSettings Settings { get; }

        /// <summary>
        /// Gets the component container, which holds components that contribute to Ninject.
        /// </summary>
        IComponentContainer Components { get; }

        /// <summary>
        /// Gets the modules that have been loaded into the kernel.
        /// </summary>
        /// <returns>A series of loaded modules.</returns>
        IEnumerable<INinjectModule> GetModules();

        /// <summary>
        /// Determines whether a module with the specified name has been loaded in the kernel.
        /// </summary>
        /// <param name="name">The name of the module.</param>
        /// <returns><c>True</c> if the specified module has been loaded; otherwise, <c>false</c>.</returns>
        bool HasModule(string name);

        /// <summary>
        /// Loads the module(s) into the kernel.
        /// </summary>
        /// <param name="m">The modules to load.</param>
        void Load(IEnumerable<INinjectModule> m);

        /// <summary>
        /// Loads modules from the files that match the specified pattern(s).
        /// </summary>
        /// <param name="filePatterns">The file patterns (i.e. "*.dll", "modules/*.rb") to match.</param>
        void Load(IEnumerable<string> filePatterns);

        /// <summary>
        /// Loads modules defined in the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to search.</param>
        void Load(IEnumerable<Assembly> assemblies);

        /// <summary>
        /// Unloads the plugin with the specified name.
        /// </summary>
        /// <param name="name">The plugin's name.</param>
        void Unload(string name);

        /// <summary>
        /// Gets the bindings registered for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>A series of bindings that are registered for the service.</returns>
        IEnumerable<IBinding> GetBindings(Type service);

        /// <summary>
        /// Begins a new activation block, which can be used to deterministically dispose resolved instances.
        /// </summary>
        /// <returns>The new activation block.</returns>
        IActivationBlock BeginBlock();
    }
}