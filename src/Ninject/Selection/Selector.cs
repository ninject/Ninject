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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Selection.Heuristics;
#endregion

namespace Ninject.Selection
{
    using Ninject.Infrastructure.Language;

    /// <summary>
    /// Selects members for injection.
    /// </summary>
    public class Selector : NinjectComponent, ISelector
    {
        /// <summary>
        /// Gets or sets the constructor scorer.
        /// </summary>
        public IConstructorScorer ConstructorScorer { get; set; }

        /// <summary>
        /// Gets the property injection heuristics.
        /// </summary>
        public ICollection<IInjectionHeuristic> InjectionHeuristics { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// </summary>
        /// <param name="constructorScorer">The constructor scorer.</param>
        /// <param name="injectionHeuristics">The injection heuristics.</param>
        public Selector(IConstructorScorer constructorScorer, IEnumerable<IInjectionHeuristic> injectionHeuristics)
        {
            ConstructorScorer = constructorScorer;
            InjectionHeuristics = injectionHeuristics.ToList();
        }

        /// <summary>
        /// Selects the constructor to call on the specified type, by using the constructor scorer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The selected constructor, or <see langword="null"/> if none were available.</returns>
        public  virtual IEnumerable<ConstructorInfo> SelectConstructorsForInjection(Type type)
        {
            if (type.GetTypeInfo().IsSubclassOf(typeof(MulticastDelegate)))
                return null;
            var tInfo = type.GetTypeInfo();
            var constructors = tInfo.DeclaredConstructors.FilterPublic(Settings.InjectNonPublic);
            return constructors.Any() ? constructors : null;
        }

        /// <summary>
        /// Selects properties that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected properties.</returns>
        public virtual IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            
            properties.AddRange(
                type.GetRuntimeProperties().FilterPublic(Settings.InjectNonPublic)
                    .Select(p => p.GetPropertyFromDeclaredType(p))
                    .Where(p => this.InjectionHeuristics.Any(h => p != null && h.ShouldInject(p))));


            if (this.Settings.InjectParentPrivateProperties)
            {
                for (Type parentType = type.GetTypeInfo().BaseType; parentType != null; parentType = parentType.GetTypeInfo().BaseType)
                {
                    properties.AddRange(this.GetPrivateProperties(type.GetTypeInfo().BaseType));
                }
            }
            
            return properties;
        }
        
        private IEnumerable<PropertyInfo> GetPrivateProperties(Type type)
        {
            return type.GetRuntimeProperties().FilterPublic(Settings.InjectNonPublic).Where(p => p.DeclaringType == type && p.IsPrivate())
                .Where(p => this.InjectionHeuristics.Any(h => h.ShouldInject(p)));
        }

        /// <summary>
        /// Selects methods that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected methods.</returns>
        public virtual IEnumerable<MethodInfo> SelectMethodsForInjection(Type type)
        {
            return type.GetRuntimeMethods().FilterPublic(Settings.InjectNonPublic).Where(m => InjectionHeuristics.Any(h => h.ShouldInject(m)));
        }
    }
}

namespace Ninject
{

    internal static class WinRTFilters
    {
        public static IEnumerable<T> FilterPublic<T>(this IEnumerable<T> input, bool nonPublic)
            where T : MethodBase
        {
            return input.Where(m => !m.IsStatic && (nonPublic || m.IsPublic));
        }

        public static IEnumerable<PropertyInfo> FilterPublic(this IEnumerable<PropertyInfo> input, bool nonPublic)
       {
           var toReturn = from pi in input
                          let method = pi.SetMethod ?? pi.GetMethod
                          where !method.IsStatic && (nonPublic || method.IsPublic)
                          select pi;

           return toReturn;
        }
    }

}
