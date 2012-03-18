//-------------------------------------------------------------------------------
// <copyright file="IBindingRoot.cs" company="Ninject Project Contributors">
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

namespace Ninject.Syntax
{
    using System;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// Provides a path to register bindings.
    /// </summary>
    public interface IBindingRoot : IFluentSyntax
    {
        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T> Bind<T>();

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2> Bind<T1, T2>();

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>();

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <typeparam name="T4">The fourth service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>();

        /// <summary>
        /// Declares a binding from the service to itself.
        /// </summary>
        /// <param name="services">The services to bind.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<object> Bind(params Type[] services);

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to unbind.</typeparam>
        void Unbind<T>();

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        void Unbind(Type service);

        /// <summary>
        /// Removes any existing bindings for the specified service, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1> Rebind<T1>();

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2> Rebind<T1, T2>();

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>();

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <typeparam name="T4">The fourth service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>();

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <param name="services">The services to re-bind.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingToSyntax<object> Rebind(params Type[] services);

        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        void AddBinding(IBinding binding);

        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        void RemoveBinding(IBinding binding);
    }
}