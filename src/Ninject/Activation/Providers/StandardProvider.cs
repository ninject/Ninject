using System;
using System.Linq;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

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
		/// Gets or sets the pipeline component.
		/// </summary>
		public IPipeline Pipeline { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardProvider"/> class.
		/// </summary>
		/// <param name="type">The type (or prototype) of instances the provider creates.</param>
		/// <param name="injectorFactory">The injector factory component.</param>
		/// <param name="planner">The planner component.</param>
		/// <param name="pipeline">The pipeline component.</param>
		public StandardProvider(Type type, IInjectorFactory injectorFactory, IPlanner planner, IPipeline pipeline)
		{
			Type = type;
			InjectorFactory = injectorFactory;
			Planner = planner;
			Pipeline = pipeline;
		}

		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		public virtual object Create(IContext context)
		{
			context.Plan = Planner.GetPlan(GetImplementationType(context.Request.Service));

			var directive = context.Plan.GetOne<ConstructorInjectionDirective>();

			if (directive == null)
				throw new InvalidOperationException();

			var injector = InjectorFactory.GetConstructorInjector(directive.Member);
			object[] arguments = directive.Targets.Select(target => GetValue(context, target)).ToArray();

			context.Instance = injector.Invoke(arguments);

			Pipeline.Activate(context);

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
			return ctx => new StandardProvider(prototype,
				ctx.Kernel.Components.Get<IInjectorFactory>(),
				ctx.Kernel.Components.Get<IPlanner>(),
				ctx.Kernel.Components.Get<IPipeline>());
		}
	}
}