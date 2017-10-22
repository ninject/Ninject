// -------------------------------------------------------------------------------------------------
// <copyright file="IBindingWithSyntax.cs" company="Ninject Project Contributors">
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

    using Ninject.Activation;
    using Ninject.Parameters;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Used to add additional information to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingWithSyntax<T> : IBindingSyntax
    {
        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, object value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="name">The name of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(string name, Func<IContext, ITarget, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">Specifies the argument type to override.</typeparam>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument<TValue>(TValue value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="value">The value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(Type type, object value);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument<TValue>(Func<IContext, TValue> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(Type type, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the argument to override.</typeparam>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument<TValue>(Func<IContext, ITarget, TValue> callback);

        /// <summary>
        /// Indicates that the specified constructor argument should be overridden with the specified value.
        /// </summary>
        /// <param name="type">The type of the argument to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the argument.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithConstructorArgument(Type type, Func<IContext, ITarget, object> callback);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="value">The value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithPropertyValue(string name, object value);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, object> callback);

        /// <summary>
        /// Indicates that the specified property should be injected with the specified value.
        /// </summary>
        /// <param name="name">The name of the property to override.</param>
        /// <param name="callback">The callback to invoke to get the value for the property.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithPropertyValue(string name, Func<IContext, ITarget, object> callback);

        /// <summary>
        /// Adds a custom parameter to the binding.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithParameter(IParameter parameter);

        /// <summary>
        /// Sets the value of a piece of metadata on the binding.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        /// <returns>The fluent syntax.</returns>
        IBindingWithOrOnSyntax<T> WithMetadata(string key, object value);
    }
}