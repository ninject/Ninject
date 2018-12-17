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

    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Provides a path to register bindings.
    /// </summary>
    public abstract class BindingRoot : DisposableObject, IBindingRoot
    {
        /// <summary>
        /// Gets or sets the component container, which holds components that contribute to Ninject.
        /// </summary>
        public IComponentContainer Components { get; internal protected set; }

        /// <summary>
        /// Gets or sets the ninject settings.
        /// </summary>
        protected internal INinjectSettings Settings { get; set; }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T">The service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T> Bind<T>()
        {
            var service = typeof(T);

            var binding = new Binding(service);
            binding.BindingConfiguration.ScopeCallback = this.Settings.DefaultScopeCallback;

            this.AddBinding(binding);

            return new BindingBuilder<T>(binding, this.Components.Get<IPlanner>(), this.Components.Get<IConstructorScorer>(), service.Format());
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2> Bind<T1, T2>()
        {
            var firstBinding = new Binding(typeof(T1));
            firstBinding.BindingConfiguration.ScopeCallback = this.Settings.DefaultScopeCallback;

            this.AddBinding(firstBinding);
            this.AddBinding(new Binding(typeof(T2), firstBinding.BindingConfiguration));
            var serviceNames = new[] { typeof(T1).Format(), typeof(T2).Format() };

            return new BindingBuilder<T1, T2>(firstBinding.BindingConfiguration, this.Components.Get<IPlanner>(), this.Components.Get<IConstructorScorer>(), string.Join(", ", serviceNames));
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
        {
            var firstBinding = new Binding(typeof(T1));
            firstBinding.BindingConfiguration.ScopeCallback = this.Settings.DefaultScopeCallback;

            this.AddBinding(firstBinding);
            this.AddBinding(new Binding(typeof(T2), firstBinding.BindingConfiguration));
            this.AddBinding(new Binding(typeof(T3), firstBinding.BindingConfiguration));
            var serviceNames = new[] { typeof(T1).Format(), typeof(T2).Format(), typeof(T3).Format() };

            return new BindingBuilder<T1, T2, T3>(firstBinding.BindingConfiguration, this.Components.Get<IPlanner>(), this.Components.Get<IConstructorScorer>(), string.Join(", ", serviceNames));
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <typeparam name="T1">The first service to bind.</typeparam>
        /// <typeparam name="T2">The second service to bind.</typeparam>
        /// <typeparam name="T3">The third service to bind.</typeparam>
        /// <typeparam name="T4">The fourth service to bind.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
        {
            var firstBinding = new Binding(typeof(T1));
            firstBinding.BindingConfiguration.ScopeCallback = this.Settings.DefaultScopeCallback;

            this.AddBinding(firstBinding);
            this.AddBinding(new Binding(typeof(T2), firstBinding.BindingConfiguration));
            this.AddBinding(new Binding(typeof(T3), firstBinding.BindingConfiguration));
            this.AddBinding(new Binding(typeof(T4), firstBinding.BindingConfiguration));
            var serviceNames = new[] { typeof(T1).Format(), typeof(T2).Format(), typeof(T3).Format(), typeof(T4).Format() };

            return new BindingBuilder<T1, T2, T3, T4>(firstBinding.BindingConfiguration, this.Components.Get<IPlanner>(), this.Components.Get<IConstructorScorer>(), string.Join(", ", serviceNames));
        }

        /// <summary>
        /// Declares a binding for the specified service.
        /// </summary>
        /// <param name="services">The services to bind.</param>
        /// <returns>The fluent syntax.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="services"/> contains zero types to bind.</exception>
        public IBindingToSyntax<object> Bind(params Type[] services)
        {
            Ensure.ArgumentNotNull(services, nameof(services));

            if (services.Length == 0)
            {
                throw new ArgumentException("Specify at least one type to bind.", nameof(services));
            }

            var firstBinding = new Binding(services[0]);
            firstBinding.BindingConfiguration.ScopeCallback = this.Settings.DefaultScopeCallback;

            this.AddBinding(firstBinding);

            foreach (var service in services.Skip(1))
            {
                this.AddBinding(new Binding(service, firstBinding.BindingConfiguration));
            }

            return new BindingBuilder<object>(firstBinding, this.Components.Get<IPlanner>(), this.Components.Get<IConstructorScorer>(), string.Join(", ", services.Select(service => service.Format()).ToArray()));
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
        /// <returns>The fluent syntax.</returns>
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
        /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="services"/> contains zero items.</exception>
        public IBindingToSyntax<object> Rebind(params Type[] services)
        {
            Ensure.ArgumentNotNull(services, nameof(services));

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
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        public abstract void AddBinding(IBinding binding);

        /// <summary>
        /// Unregisters the specified binding.
        /// </summary>
        /// <param name="binding">The binding to remove.</param>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        public abstract void RemoveBinding(IBinding binding);
    }
}