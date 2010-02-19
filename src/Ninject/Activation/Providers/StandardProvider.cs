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
using Ninject.Infrastructure.Language;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
using Ninject.Selection;

#endregion

namespace Ninject.Activation.Providers
{
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
		public ISelector Selector { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardProvider"/> class.
		/// </summary>
		/// <param name="type">The type (or prototype) of instances the provider creates.</param>
		/// <param name="planner">The planner component.</param>
		/// <param name="selector">The selector component.</param>
		public StandardProvider(Type type, IPlanner planner, ISelector selector)
		{
			Ensure.ArgumentNotNull(type, "type");
			Ensure.ArgumentNotNull(planner, "planner");
			Ensure.ArgumentNotNull(selector, "selector");

			Type = type;
			Planner = planner;
			Selector = selector;
		}

		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		public virtual object Create(IContext context)
		{
			Ensure.ArgumentNotNull(context, "context");

			if (context.Plan == null)
				context.Plan = Planner.GetPlan(GetImplementationType(context.Request.Service));

			if (!context.Plan.Has<ConstructorInjectionDirective>())
				throw new ActivationException(ExceptionFormatter.NoConstructorsAvailable(context));

			var directives = context.Plan.GetAll<ConstructorInjectionDirective>();
			var directive = directives.OrderByDescending(option => Selector.ConstructorScorer.Score(context, option)).First();
			object[] arguments = directive.Targets.Select(target => GetValue(context, target)).ToArray();
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
			Ensure.ArgumentNotNull(context, "context");
			Ensure.ArgumentNotNull(target, "target");

			var parameter = context.Parameters.OfType<ConstructorArgument>().Where(p => p.Name == target.Name).SingleOrDefault();
			return parameter != null ? parameter.GetValue(context) : target.ResolveWithin(context);
		}

		/// <summary>
		/// Gets the implementation type that the provider will activate an instance of
		/// for the specified service.
		/// </summary>
		/// <param name="service">The service in question.</param>
		/// <returns>The implementation type that will be activated.</returns>
		public Type GetImplementationType(Type service)
		{
			Ensure.ArgumentNotNull(service, "service");
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
			Ensure.ArgumentNotNull(prototype, "prototype");
			return ctx => new StandardProvider(prototype, ctx.Kernel.Components.Get<IPlanner>(), ctx.Kernel.Components.Get<ISelector>());
		}
	}
}