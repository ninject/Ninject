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
using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure;
#endregion

namespace Ninject.Planning.Targets.Strategies
{
    /// <summary>
    /// Interface representing a strategy for resolving targets.
    /// </summary>
    public interface ITargetResolutionStrategy : INinjectComponent
    {
        /// <summary>
        /// Resolves the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <returns>The resolved value if resolution was successful; otherwise <i>Maybe&lt;object>.None</i></returns>
        Maybe<object> Resolve(ITarget target, IContext context);
    }
}
