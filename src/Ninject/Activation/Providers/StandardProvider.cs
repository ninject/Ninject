#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Linq;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Introspection;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;
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
		/// Gets or sets the injector factory component.
		/// </summary>
		public IInjectorFactory InjectorFactory { get; private set; }

		/// <summary>
		/// Gets or sets the planner component.
		/// </summary>
		public IPlanner Planner { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardProvider"/> class.
		/// </summary>
		/// <param name="type">The type (or prototype) of instances the provider creates.</param>
		/// <param name="injectorFactory">The injector factory component.</param>
		/// <param name="planner">The planner component.</param>
		public StandardProvider(Type type, IInjectorFactory injectorFactory, IPlanner planner)
		{
			Ensure.ArgumentNotNull(type, "type");
			Ensure.ArgumentNotNull(injectorFactory, "injectorFactory");
			Ensure.ArgumentNotNull(planner, "planner");

			Type = type;
			InjectorFactory = injectorFactory;
			Planner = planner;
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

			var directive = context.Plan.GetOne<ConstructorInjectionDirective>();

			if (directive == null)
				throw new ActivationException(ExceptionFormatter.NoConstructorsAvailable(context));

			var injector = InjectorFactory.GetConstructorInjector(directive.Member);
			object[] arguments = directive.Targets.Select(target => GetValue(context, target)).ToArray();

			context.Instance = injector.Invoke(arguments);

			return context.Instance;
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

			return ctx => new StandardProvider(prototype,
				ctx.Kernel.Components.Get<IInjectorFactory>(),
				ctx.Kernel.Components.Get<IPlanner>());
		}
	}
}