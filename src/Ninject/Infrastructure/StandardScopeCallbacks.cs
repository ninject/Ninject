// -------------------------------------------------------------------------------------------------
// <copyright file="StandardScopeCallbacks.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure
{
    using System;
    using Ninject.Activation;

    /// <summary>
    /// Scope callbacks for standard scopes.
    /// </summary>
    public class StandardScopeCallbacks
    {
        /// <summary>
        /// Gets the callback for transient scope.
        /// </summary>
        public static readonly Func<IContext, object> Transient = ctx => null;

        /// <summary>
        /// Gets the callback for singleton scope.
        /// </summary>
        public static readonly Func<IContext, object> Singleton = ctx => ctx.Kernel;

#if !NO_THREAD_SCOPE
        /// <summary>
        /// Gets the callback for thread scope.
        /// </summary>
        public static readonly Func<IContext, object> Thread = ctx => System.Threading.Thread.CurrentThread;
#endif
    }
}