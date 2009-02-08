using System;
using System.Linq;
using Ninject.Injection;
using Ninject.Planning.Directives;

namespace Ninject.Activation.Strategies
{
	public class MethodInjectionStrategy : ActivationStrategy
	{
		public IInjectorFactory InjectorFactory { get; set; }

		public MethodInjectionStrategy(IInjectorFactory injectorFactory)
		{
			InjectorFactory = injectorFactory;
		}

		public override void Activate(IContext context)
		{
			foreach (var directive in context.Plan.GetAll<MethodInjectionDirective>())
			{
				var injector = InjectorFactory.GetMethodInjector(directive.Member);
				var arguments = directive.Targets.Select(target => target.ResolveWithin(context));
				injector.Invoke(context.Instance, arguments.ToArray());
			}
		}
	}
}