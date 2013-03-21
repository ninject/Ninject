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
using System.Linq;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Ninject.Selection;

#endregion

namespace Ninject.Activation.Providers
{
    using System.Reflection;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// The standard provider for types, which activates instances via a <see cref="IPipeline"/>.
    /// </summary>
    public class StandardProvider : IProvider
    {
        /// <summary>
        /// Gets the type (or prototype) of instances the provider creates.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets or sets the planner component.
        /// </summary>
        public IPlanner Planner { get; private set; }

        /// <summary>
        /// Gets or sets the selector component.
        /// </summary>
        public IConstructorScorer ConstructorScorer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardProvider"/> class.
        /// </summary>
        /// <param name="type">The type (or prototype) of instances the provider creates.</param>
        /// <param name="planner">The planner component.</param>
        /// <param name="constructorScorer">The constructor scorer component.</param>
        public StandardProvider(Type type, IPlanner planner, IConstructorScorer constructorScorer
            )
        {
            Type = type;
            Planner = planner;
            ConstructorScorer = constructorScorer;
        }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        public virtual object Create(IContext context)
        {
            if (context.Plan == null)
            {
                context.Plan = this.Planner.GetPlan(this.GetImplementationType(context.Request.Service));
            }


            var directives = context.Plan.ConstructorInjectionDirectives;
            ConstructorInjectionDirective directive = null;
            if (directives.Count == 1)
            {
                directive = directives[0];
            }
            else
            {
                var bestDirectives = directives
                      .GroupBy(option => this.ConstructorScorer.Score(context, option))
                      .OrderByDescending(g => g.Key)
                      .FirstOrDefault();
                if (bestDirectives == null || !bestDirectives.Any())
                {
                    throw new ActivationException(ExceptionFormatter.NoConstructorsAvailable(context));
                }

                if (bestDirectives.Skip(1).Any())
                {
                    throw new ActivationException(ExceptionFormatter.ConstructorsAmbiguous(context, bestDirectives));
                }         
        
                directive = bestDirectives.Single();
            }
  
            var arguments = directive.Targets.Select(target => this.GetValue(context, target)).ToArray();
            return directive.Injector(arguments);
        }

        /// <summary>
        /// Gets the value to inject into the specified target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>The value to inject into the specified target.</returns>
        public object GetValue(IContext context, ITarget target)
        {
            var parameter = context
                .Parameters.OfType<IConstructorArgument>()
                .Where(p => p.AppliesToTarget(context, target)).SingleOrDefault();
            return parameter != null ? parameter.GetValue(context, target) : target.ResolveWithin(context);
        }

        /// <summary>
        /// Gets the implementation type that the provider will activate an instance of
        /// for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>The implementation type that will be activated.</returns>
        public Type GetImplementationType(Type service)
        {
            return Type.ContainsGenericParameters ? Type.MakeGenericType(service.GetGenericArguments()) : Type;
        }

        /// <summary>
        /// Gets a callback that creates an instance of the <see cref="StandardProvider"/>
        /// for the specified type.
        /// </summary>
        /// <param name="prototype">The prototype the provider instance will create.</param>
        /// <returns>The created callback.</returns>
        public static Func<IContext, IProvider> GetCreationCallback(Type prototype)
        {
            return ctx => new StandardProvider(prototype, ctx.Kernel.Planner, ctx.Kernel.Components.Get<ISelector>().ConstructorScorer);
        }

        /// <summary>
        /// Gets a callback that creates an instance of the <see cref="StandardProvider"/>
        /// for the specified type and constructor.
        /// </summary>
        /// <param name="prototype">The prototype the provider instance will create.</param>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created callback.</returns>
        public static Func<IContext, IProvider> GetCreationCallback(Type prototype, ConstructorInfo constructor)
        {
            return ctx => new StandardProvider(prototype, ctx.Kernel.Planner, new SpecificConstructorSelector(constructor));
        }
    }
}