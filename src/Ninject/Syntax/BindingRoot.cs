// -------------------------------------------------------------------------------------------------
// <copyright file="BindingRoot.cs" company="Ninject Project Contributors">
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

namespace Ninject.Syntax
{
    using System;
    using System.Linq;

    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// Provides a path to register bindings.
    /// </summary>
    public abstract class BindingRoot : DisposableObject, IBindingRoot
    {
        /// <summary>
        /// Gets the kernel.
        /// </summary>
        /// <value>The kernel.</value>
        protected abstract IKernel KernelInstance { get; }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T> Bind<T>()
        {
            var service = typeof(T);
            var bindingConfiguration = new BindingConfiguration { ScopeCallback = this.KernelInstance.Settings.DefaultScopeCallback };

            var binding = new Binding(service, bindingConfiguration);
            this.AddBinding(binding);

            return new BindingBuilder<T>(binding, service.Format());
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T1, T2> Bind<T1, T2>()
        {
            var service1 = typeof(T1);
            var service2 = typeof(T2);
            var bindingConfiguration = new BindingConfiguration { ScopeCallback = this.KernelInstance.Settings.DefaultScopeCallback };

            this.AddBinding(new Binding(service1, bindingConfiguration));
            this.AddBinding(new Binding(service2, bindingConfiguration));

            var serviceNames = new[] { service1.Format(), service2.Format() };

            return new BindingBuilder<T1, T2>(bindingConfiguration, string.Join(", ", serviceNames));
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
        {
            var service1 = typeof(T1);
            var service2 = typeof(T2);
            var service3 = typeof(T3);
            var bindingConfiguration = new BindingConfiguration { ScopeCallback = this.KernelInstance.Settings.DefaultScopeCallback };

            this.AddBinding(new Binding(service1, bindingConfiguration));
            this.AddBinding(new Binding(service2, bindingConfiguration));
            this.AddBinding(new Binding(service3, bindingConfiguration));

            var serviceNames = new[] { typeof(T1).Format(), typeof(T2).Format(), typeof(T3).Format() };

            return new BindingBuilder<T1, T2, T3>(bindingConfiguration, string.Join(", ", serviceNames));
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <typeparam name="T4">The fourth service to bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
        {
            var service1 = typeof(T1);
            var service2 = typeof(T2);
            var service3 = typeof(T3);
            var service4 = typeof(T4);
            var bindingConfiguration = new BindingConfiguration { ScopeCallback = this.KernelInstance.Settings.DefaultScopeCallback };

            this.AddBinding(new Binding(service1, bindingConfiguration));
            this.AddBinding(new Binding(service2, bindingConfiguration));
            this.AddBinding(new Binding(service3, bindingConfiguration));
            this.AddBinding(new Binding(service4, bindingConfiguration));

            var serviceNames = new[] { typeof(T1).Format(), typeof(T2).Format(), typeof(T3).Format(), typeof(T4).Format() };

            return new BindingBuilder<T1, T2, T3, T4>(bindingConfiguration, string.Join(", ", serviceNames));
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <param name="services">The services to bind.</param>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<object> Bind(params Type[] services)
        {
            Ensure.ArgumentNotNull(services, "services");
            if (services.Length == 0)
            {
                throw new ArgumentException("The services must contain at least one type", "services");
            }

            var bindingConfiguration = new BindingConfiguration { ScopeCallback = this.KernelInstance.Settings.DefaultScopeCallback };

            var firstBinding = new Binding(services[0], bindingConfiguration);
            this.AddBinding(firstBinding);

            foreach (var service in services.Skip(1))
            {
                this.AddBinding(new Binding(service, bindingConfiguration));
            }

            return new BindingBuilder<object>(firstBinding, string.Join(", ", services.Select(service => service.Format()).ToArray()));
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <param name="service">The service to bind.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<object> Bind(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");
            var bindingConfiguration = new BindingConfiguration { ScopeCallback = this.KernelInstance.Settings.DefaultScopeCallback };

            var binding = new Binding(service, bindingConfiguration);
            this.AddBinding(binding);

            return new BindingBuilder<object>(binding, service.Format());
        }

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to unbind.</typeparam>
        public void Unbind<T>()
        {
            this.Unbind(typeof(T));
        }

        /// <summary>
        /// Unregisters all bindings for the specified service.
        /// </summary>
        /// <param name="service">The service to unbind.</param>
        public abstract void Unbind(Type service);

        /// <summary>
        /// Removes any existing bindings for the specified service, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <returns>The fluent syntax</returns>
        public IBindingToSyntax<T1> Rebind<T1>()
        {
            this.Unbind<T1>();
            return this.Bind<T1>();
        }

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2> Rebind<T1, T2>()
        {
            this.Unbind<T1>();
            this.Unbind<T2>();
            return this.Bind<T1, T2>();
        }

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>()
        {
            this.Unbind<T1>();
            this.Unbind<T2>();
            this.Unbind<T3>();
            return this.Bind<T1, T2, T3>();
        }

        /// <summary>
        /// Removes any existing bindings for the specified services, and declares a new one.
        /// </summary>
        /// <typeparam name="T1">The first service to re-bind.</typeparam>
        /// <typeparam name="T2">The second service to re-bind.</typeparam>
        /// <typeparam name="T3">The third service to re-bind.</typeparam>
        /// <typeparam name="T4">The fourth service to re-bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>()
        {
            this.Unbind<T1>();
            this.Unbind<T2>();
            this.Unbind<T3>();
            this.Unbind<T4>();
            return this.Bind<T1, T2, T3, T4>();
        }

        /// <summary>
        /// Removes any existing bindings for the specified service, and declares a new one.
        /// </summary>
        /// <param name="services">The services to re-bind.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<object> Rebind(params Type[] services)
        {
            foreach (var service in services)
            {
                this.Unbind(service);
            }

            return this.Bind(services);
        }

        /// <summary>
        /// Registers the specified binding.
        /// </summary>
        /// <param name="binding">The binding to add.</param>
        public abstract void AddBinding(IBinding binding);

        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        public abstract void RemoveBinding(IBinding binding);
    }
}