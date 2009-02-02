using System;
using System.Linq;
using Ninject.Injection;
using Ninject.Planning.Directives;

namespace Ninject.Activation.Strategies
{
	public class MethodInjectionStrategy : ActivationStrategy
	{
		public IKernel Kernel { get; set; }
		public IInjectorFactory InjectorFactory { get; set; }

		public MethodInjectionStrategy(IKernel kernel, IInjectorFactory injectorFactory)
		{
			Kernel = kernel;
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