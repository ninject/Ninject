#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion

namespace Ninject.Activation
{
    using System;
    using Ninject.Infrastructure;

    /// <summary>
    /// A simple abstract provider for instances of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of instances the provider creates.</typeparam>
    public abstract class Provider<T> : IProvider<T>
    {
        /// <summary>
        /// Gets the type (or prototype) of instances the provider creates.
        /// </summary>
        public virtual Type Type
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        public object Create(IContext context)
        {
            Ensure.ArgumentNotNull(context, "context");
            return this.CreateInstance(context);
        }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        protected abstract T CreateInstance(IContext context);
    }
}