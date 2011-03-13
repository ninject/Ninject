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
using System.Collections;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;
#endregion

namespace Ninject.Planning.Targets.Strategies
{
    /// <summary>
    /// Class that resolves requests for enumerables using the Ninkect kernel.
    /// </summary>
    public class EnumerableTargetResolutionStrategy : NinjectComponent, ITargetResolutionStrategy
    {
        /// <summary>
        /// Resolves the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <returns>The resolved value if resolution was successful; otherwise <i>Maybe&lt;object>.None</i></returns>
        public Maybe<object> Resolve(ITarget target, IContext context)
        {
            var type = target.Type;
            Func<IEnumerable, IEnumerable> transform = null;
            Type service = null;
            if (type.IsArray)
            {
                service = type.GetElementType();
                transform = r => r.ToArraySlow(service);
            }
            else if (type.IsGenericType)
            {
                var gtd = type.GetGenericTypeDefinition();
                service = type.GetGenericArguments()[0];

                if (gtd == typeof(List<>) || gtd == typeof(IList<>) || gtd == typeof(ICollection<>))
                {
                    transform = r => r.ToListSlow(service);
                }
                else if (gtd == typeof(IEnumerable<>))
                {
                    transform = r => r;
                }
            }

            if (transform != null && service != null)
            {
                var request = context.Request.CreateChild(service, context, target);
                request.IsOptional = true;
                return new Maybe<object>(transform(context.Kernel.Resolve(request).CastSlow(service)));
            }
            
            return Maybe<object>.None;
        }
    }
}