// -------------------------------------------------------------------------------------------------
// <copyright file="CallbackProvider.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Providers
{
    using System;
    using Ninject.Infrastructure;

    /// <summary>
    /// A provider that delegates to a callback method to create instances.
    /// </summary>
    /// <typeparam name="T">The type of instances the provider creates.</typeparam>
    public class CallbackProvider<T> : Provider<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackProvider{T}"/> class.
        /// </summary>
        /// <param name="method">The callback method that will be called to create instances.</param>
        public CallbackProvider(Func<IContext, T> method)
        {
            Ensure.ArgumentNotNull(method, "method");

            this.Method = method;
        }

        /// <summary>
        /// Gets the callback method used by the provider.
        /// </summary>
        public Func<IContext, T> Method { get; private set; }

        /// <summary>
        /// Invokes the callback method to create an instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        protected override T CreateInstance(IContext context)
        {
            return this.Method(context);
        }
    }
}