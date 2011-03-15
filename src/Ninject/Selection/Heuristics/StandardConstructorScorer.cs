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
using System.Linq;
using System.Reflection;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;
using Ninject.Parameters;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

#endregion

namespace Ninject.Selection.Heuristics
{
    /// <summary>
    /// Scores constructors by either looking for the existence of an injection marker
    /// attribute, or by counting the number of parameters.
    /// </summary>
    public class StandardConstructorScorer : NinjectComponent, IConstructorScorer
    {
        /// <summary>
        /// Gets the score for the specified constructor.
        /// </summary>
        /// <param name="context">The injection context.</param>
        /// <param name="directive">The constructor.</param>
        /// <returns>The constructor's score.</returns>
        public int Score(IContext context, ConstructorInjectionDirective directive)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(directive, "constructor");
            
            if(directive.Constructor.HasAttribute(Settings.InjectAttribute))
                return Int32.MaxValue;
            
            int score = 1;
            foreach(ITarget target in directive.Targets)
            {
                foreach(IParameter parameter in context.Parameters)
                {
                    if(string.Equals(target.Name, parameter.Name))
                    {
                        score++;
                        continue;
                    }
                }
                
                Type targetType = target.Type;
                if(targetType.IsArray)
                    targetType = targetType.GetElementType();

                if(targetType.IsGenericType && targetType.GetInterfaces().Any(type => type == typeof(IEnumerable)))
                    targetType = targetType.GetGenericArguments()[0];
                
                if(context.Kernel.GetBindings(targetType).Any())
                    score++;
            }
            
            return score;
        }
    }
}
