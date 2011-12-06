//-------------------------------------------------------------------------------
// <copyright file="IBindingToSyntax.cs" company="Ninject Project Contributors">
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
#if !NETCF
    using System.Linq.Expressions;
#endif
    using Ninject.Activation;

    /// <summary>
    /// Used to define the target of a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingToSyntax<T> : IBindingSyntax
    {
        /// <summary>
        /// Indicates that the service should be self-bound.
        /// </summary>
        /// <returns>The fluent syntax.</returns>
        IBindingWhenInNamedWithOrOnSyntax<T> ToSelf();

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingWhenInNamedWithOrOnSyntax<T> To<TImplementation>() 
            where TImplementation : T;

        /// <summary>
        /// Indicates that the service should be bound to the specified implementation type.
        /// </summary>
        /// <param name="implementation">The implementation type.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWhenInNamedWithOrOnSyntax<T> To(Type implementation);

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <typeparam name="TProvider">The type of provider to activate.</typeparam>
        /// <returns>The fluent syntax.</returns>
        IBindingWhenInNamedWithOrOnSyntax<T> ToProvider<TProvider>() where TProvider : IProvider;

        /// <summary>
        /// Indicates that the service should be bound to an instance of the specified provider type.
        /// The instance will be activated via the kernel when an instance of the service is activated.
        /// </summary>
        /// <param name="providerType">The type of provider to activate.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWhenInNamedWithOrOnSyntax<T> ToProvider(Type providerType);

        /// <summary>
        /// Indicates that the service should be bound to the specified provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWhenInNamedWithOrOnSyntax<T> ToProvider(IProvider provider);

        /// <summary>
        /// Indicates that the service should be bound to the specified callback method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWhenInNamedWithOrOnSyntax<T> ToMethod(Func<IContext, T> method);

        /// <summary>
        /// Indicates that the service should be bound to the specified constant value.
        /// </summary>
        /// <param name="value">The constant value.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWhenInNamedWithOrOnSyntax<T> ToConstant(T value);

#if !NETCF
        /// <summary>
        /// Indicates that the service should be bound to the speecified constructor.
        /// </summary>
        /// <param name="newExpression">The expression that specifies the constructor.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWhenInNamedWithOrOnSyntax<T> ToConstructor(Expression<Func<IConstructorArgumentSyntax, T>> newExpression);
#endif
    }
}