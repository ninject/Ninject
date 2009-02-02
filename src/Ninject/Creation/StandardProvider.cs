using System;
using System.Linq;
using Ninject.Activation;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

namespace Ninject.Creation
{
	public class StandardProvider : IProvider
	{
		public IKernel Kernel { get; set; }
		public IInjectorFactory InjectorFactory { get; set; }

		public Type Prototype { get; private set; }

		public StandardProvider(Type prototype, IKernel kernel, IInjectorFactory injectorFactory)
		{
			Prototype = prototype;
			Kernel = kernel;
			InjectorFactory = injectorFactory;
		}

		public virtual Type GetImplementationType(IContext context)
		{
			return Prototype.ContainsGenericParameters ? Prototype.MakeGenericType(context.Request.Service.GetGenericArguments()) : Prototype;
		}

		public virtual object Create(IContext context)
		{
			var directive = context.Plan.GetOne<ConstructorInjectionDirective>();

			if (directive == null)
				throw new InvalidOperationException();

			var injector = InjectorFactory.GetConstructorInjector(directive.Member);
			return injector.Invoke(directive.Targets.Select(target => GetValue(context, target)).ToArray());
		}

		public object GetValue(IContext context, ITarget target)
		{
			var parameter = context.Parameters.OfType<ConstructorArgument>().Where(p => p.Name == target.Name).SingleOrDefault();
			return parameter != null ? parameter.GetValue(context) : target.ResolveWithin(context);
		}

		public static Func<IContext, IProvider> GetCreationCallback(Type prototype)
		{
			return ctx => new StandardProvider(prototype, ctx.Kernel, ctx.Kernel.Components.Get<IInjectorFactory>());
		}
	}
}