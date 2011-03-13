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
using System.Linq;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Infrastructure.Language;
#endregion
namespace Ninject.Planning.Targets.Strategies
{
    /// <summary>
    /// Class that resolves targets from the Ninject kernel.
    /// </summary>
    public class KernelTargetResolutionStrategy : NinjectComponent, ITargetResolutionStrategy
    {
        /// <summary>
        /// Resolves the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <returns>The resolved value if resolution was successful; otherwise <i>Maybe&lt;object>.None</i></returns>
        public Maybe<object> Resolve(ITarget target, IContext context)
        {
            var request = context.Request.CreateChild(target.Type, context, target);
            request.IsOptional = true;

            var result = context.Kernel.Resolve(request).ToList();
            if(result.Count() > 1)
            {
                throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveBinding(request));
            }

            return result.FirstOrNone();
        }
    }
}