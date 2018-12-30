﻿// -------------------------------------------------------------------------------------------------
// <copyright file="BindingBuilder{T1}.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Bindings
{
    using System;
    using System.Linq.Expressions;

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Infrastructure;
    using Ninject.Selection.Heuristics;
    using Ninject.Syntax;

    /// <summary>
    /// Provides a root for the fluent syntax associated with an <see cref="Binding"/>.
    /// </summary>
    /// <typeparam name="T1">The service type.</typeparam>
    public class BindingBuilder<T1> : BindingBuilder, IBindingToSyntax<T1>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingBuilder{T1}"/> class.
        /// </summary>
        /// <param name="binding">The binding to build.</param>
        /// <param name="planner">The <see cref="IPlanner"/> component.</param>
        /// <param name="constructorScorer">The <see cref="IConstructorScorer"/> component.</param>
        /// <param name="serviceNames">The names of the services.</param>
        /// <exception cref="ArgumentNullException"><paramref name="binding"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="planner"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="constructorScorer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceNames"/> is <see langword="null"/>.</exception>
        public BindingBuilder(
            IBinding binding,
            IPlanner planner,
            IConstructorScorer constructorScorer,
            string serviceNames)
            : base(
                  binding.BindingConfiguration,
                  planner,
                  constructorScorer,
                  serviceNames)
        {
            Ensure.ArgumentNotNull(binding, nameof(binding));

            this.Binding = binding;
        }

        /// <summary>
        /// Gets the binding being built.
        /// </summary>
        public IBinding Binding { get; private set; }

        /// <summary>
        /// Indicates that the service should be self-bound.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<T1> ToSelf()
        {
            this.Binding.ProviderCallback = ctx => new StandardProvider(this.Binding.Service, this.Planner, this.ConstructorScorer);
            this.Binding.Target = BindingTarget.Self;

            return new BindingConfigurationBuilder<T1>(this.Binding.BindingConfiguration, this.ServiceNames);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T1
        {
            return this.InternalTo<TImplementation>();
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <param name="implementation">The implementation type.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<T1> To(Type implementation)
        {
            return this.InternalTo<T1>(implementation);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified constructor.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="newExpression">The expression that specifies the constructor.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstructor<TImplementation>(
            Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
            where TImplementation : T1
        {
            return this.InternalToConstructor(newExpression);
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<T1> ToProvider<TProvider>()
            where TProvider : IProvider
        {
            return this.ToProviderInternal<TProvider, T1>();
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <param name="providerType">The type of provider to activate.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<T1> ToProvider(Type providerType)
        {
            return this.ToProviderInternal<T1>(providerType);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProvider<TImplementation>(IProvider<TImplementation> provider)
            where TImplementation : T1
        {
            return this.InternalToProvider(provider);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<T1> ToMethod(Func<IContext, T1> method)
        {
            return this.InternalToMethod(method);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method)
            where TImplementation : T1
        {
            return this.InternalToMethod(method);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="value">The constant value.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstant<TImplementation>(TImplementation value)
            where TImplementation : T1
        {
            return this.InternalToConfiguration(value);
        }
    }
}