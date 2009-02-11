using System;
using System.Linq;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

namespace Ninject.Activation.Providers
{
	public class StandardProvider : IProvider
	{
		public Type Type { get; private set; }

		public IInjectorFactory InjectorFactory { get; private set; }
		public IPlanner Planner { get; private set; }
		public IPipeline Pipeline { get; private set; }

		public StandardProvider(Type type, IInjectorFactory injectorFactory, IPlanner planner, IPipeline pipeline)
		{
			if (planner == null)
				throw new ArgumentNullException();

			Type = type;
			InjectorFactory = injectorFactory;
			Planner = planner;
			Pipeline = pipeline;
		}

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

		public object GetValue(IContext context, ITarget target)
		{
			var parameter = context.Parameters.OfType<ConstructorArgument>().Where(p => p.Name == target.Name).SingleOrDefault();
			return parameter != null ? parameter.GetValue(context) : target.ResolveWithin(context);
		}

		private Type GetImplementationType(Type service)
		{
			return Type.ContainsGenericParameters ? Type.MakeGenericType(service.GetGenericArguments()) : Type;
		}

		public static Func<IContext, IProvider> GetCreationCallback(Type prototype)
		{
			return ctx => new StandardProvider(prototype,
				ctx.Kernel.Components.Get<IInjectorFactory>(),
				ctx.Kernel.Components.Get<IPlanner>(),
				ctx.Kernel.Components.Get<IPipeline>());
		}
	}
}