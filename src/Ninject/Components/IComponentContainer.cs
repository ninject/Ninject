// -------------------------------------------------------------------------------------------------
// <copyright file="IComponentContainer.cs" company="Ninject Project Contributors">
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

namespace Ninject.Components
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An internal container that manages and resolves components that contribute to Ninject.
    /// </summary>
    public interface IComponentContainer : IDisposable
    {
        /// <summary>
        /// Gets or sets the kernel that owns the component container.
        /// </summary>
        IKernel Kernel { get; set; }

        /// <summary>
        /// Registers a component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        void Add<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent;

        /// <summary>
        /// Removes all registrations for the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        void RemoveAll<T>()
            where T : INinjectComponent;

        /// <summary>
        /// Removes all registrations for the specified component.
        /// </summary>
        /// <param name="component">The component's type.</param>
        void RemoveAll(Type component);

        /// <summary>
        /// Removes the specified registration.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        void Remove<T, TImplementation>()
            where T : INinjectComponent
            where TImplementation : T;

        /// <summary>
        /// Gets one instance of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The instance of the component.</returns>
        T Get<T>()
            where T : INinjectComponent;

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A series of instances of the specified component.</returns>
        IEnumerable<T> GetAll<T>()
            where T : INinjectComponent;

        /// <summary>
        /// Gets one instance of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>The instance of the component.</returns>
        object Get(Type component);

        /// <summary>
        /// Gets all available instances of the specified component.
        /// </summary>
        /// <param name="component">The component type.</param>
        /// <returns>A series of instances of the specified component.</returns>
        IEnumerable<object> GetAll(Type component);

        /// <summary>
        /// Registers a transient component in the container.
        /// </summary>
        /// <typeparam name="TComponent">The component type.</typeparam>
        /// <typeparam name="TImplementation">The component's implementation type.</typeparam>
        void AddTransient<TComponent, TImplementation>()
            where TComponent : INinjectComponent
            where TImplementation : TComponent, INinjectComponent;
    }
}