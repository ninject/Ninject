#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using Ninject.Activation;
#endregion

namespace Ninject.Infrastructure
{
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

        /// <summary>
        /// Gets the callback for thread scope.
        /// </summary>
        public static readonly Func<IContext, object> Thread = ctx => System.Threading.Thread.CurrentThread;
    }
}