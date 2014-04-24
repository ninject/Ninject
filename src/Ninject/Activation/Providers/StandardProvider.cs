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

using Ninject.Infrastructure.Introspection;
using Ninject.Parameters;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

#endregion

namespace Ninject.Activation.Providers
{
    using System.Reflection;

    using Ninject.Planning.Bindings;
    using Ninject.Selection;
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
        /// Gets or sets the selector component.
        /// </summary>
        public IConstructorScorer ConstructorScorer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardProvider"/> class.
        /// </summary>
        /// <param name="type">The type (or prototype) of instances the provider creates.</param>
        /// <param name="constructorScorer">The constructor scorer component.</param>
        public StandardProvider(Type type, IConstructorScorer constructorScorer)
        {
            Type = type;
            ConstructorScorer = constructorScorer;
        }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        public virtual object Create(IContext context)
        {
            context.BuildPlan(this.GetImplementationType(context.Request.Service));
            if (!context.Plan.Has<ConstructorInjectionDirective>())
            {
                throw new ActivationException(ExceptionFormatter.NoConstructorsAvailable(context));
            }

            var directives = context.Plan.GetAll<ConstructorInjectionDirective>();
            var bestDirectives = directives
                .GroupBy(option => this.ConstructorScorer.Score(context, option))
                .OrderByDescending(g => g.Key)
                .First();
            if (bestDirectives.Skip(1).Any())
            {
                throw new ActivationException(ExceptionFormatter.ConstructorsAmbiguous(context, bestDirectives));
            }

            var directive = bestDirectives.Single();
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
                .SingleOrDefault(p => p.AppliesToTarget(context, target));
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
        /// <param name="selector">The selector</param>
        /// <returns>The created callback.</returns>
        public static Func<IContext, IProvider> GetCreationCallback(Type prototype, ISelector selector)
        {
            var provider = new StandardProvider(prototype, selector.ConstructorScorer);
            return ctx => provider;
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
            var provider = new StandardProvider(prototype, new SpecificConstructorSelector(constructor));
            return ctx => provider;
        }

        /// <summary>
        /// Assigns the provider callback to the building configuration.
        /// </summary>
        /// <param name="bindingConfiguration">The building configuration</param>
        /// <param name="prototype">The prototype</param>
        public static void AssignProviderCallback(IBindingConfiguration bindingConfiguration, Type prototype)
        {
            var provider = new StandardProvider(prototype, null);
            bindingConfiguration.ProviderCallback = ctx => provider;
            bindingConfiguration.InitializeProviderCallback =
                selector => provider.ConstructorScorer = selector.ConstructorScorer;
        }
    }
}