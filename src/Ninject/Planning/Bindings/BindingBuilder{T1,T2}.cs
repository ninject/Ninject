﻿// -------------------------------------------------------------------------------------------------
// <copyright file="BindingBuilder{T1,T2}.cs" company="Ninject Project Contributors">
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
    using Ninject.Selection.Heuristics;
    using Ninject.Syntax;

    /// <summary>
    /// Provides a root for the fluent syntax associated with an <see cref="BindingBuilder.BindingConfiguration"/>.
    /// </summary>
    /// <typeparam name="T1">The first service type.</typeparam>
    /// <typeparam name="T2">The second service type.</typeparam>
    public class BindingBuilder<T1, T2> : BindingBuilder, IBindingToSyntax<T1, T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingBuilder{T1, T2}"/> class.
        /// </summary>
        /// <param name="bindingConfiguration">The binding to build.</param>
        /// <param name="planner">The <see cref="IPlanner"/> component.</param>
        /// <param name="constructorScorer">The <see cref="IConstructorScorer"/> component.</param>
        /// <param name="serviceNames">The names of the services.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bindingConfiguration"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="planner"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="constructorScorer"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serviceNames"/> is <see langword="null"/>.</exception>
        public BindingBuilder(
            IBindingConfiguration bindingConfiguration,
            IPlanner planner,
            IConstructorScorer constructorScorer,
            string serviceNames)
            : base(
                 bindingConfiguration,
                 planner,
                 constructorScorer,
                 serviceNames)
        {
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> To<TImplementation>()
            where TImplementation : T1, T2
        {
            return this.InternalTo<TImplementation>();
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <param name="implementation">The implementation type.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<object> To(Type implementation)
        {
            return this.InternalTo<object>(implementation);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified constructor.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="newExpression">The expression that specifies the constructor.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToConstructor<TImplementation>(
            Expression<Func<IConstructorArgumentSyntax, TImplementation>> newExpression)
            where TImplementation : T1, T2
        {
            return this.InternalToConstructor(newExpression);
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<object> ToProvider<TProvider>()
            where TProvider : IProvider
        {
            return this.ToProviderInternal<TProvider, object>();
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProvider<TProvider, TImplementation>()
            where TProvider : IProvider<TImplementation>
            where TImplementation : T1, T2
        {
            return this.ToProviderInternal<TProvider, TImplementation>();
        }

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <param name="providerType">The type of provider to activate.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<object> ToProvider(Type providerType)
        {
            return this.ToProviderInternal<object>(providerType);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToProvider<TImplementation>(IProvider<TImplementation> provider)
            where TImplementation : T1, T2
        {
            return this.InternalToProvider(provider);
        }

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="method">The method.</param>
        /// <returns>The fluent syntax.</returns>
        public IBindingWhenInNamedWithOrOnSyntax<TImplementation> ToMethod<TImplementation>(Func<IContext, TImplementation> method)
            where TImplementation : T1, T2
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
            where TImplementation : T1, T2
        {
            return this.InternalToConfiguration(value);
        }
    }
}