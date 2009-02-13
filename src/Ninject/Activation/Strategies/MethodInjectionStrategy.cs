using System;
using System.Linq;
using Ninject.Injection;
using Ninject.Planning.Directives;

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// Injects methods on an instance during activation.
	/// </summary>
	public class MethodInjectionStrategy : ActivationStrategy
	{
		/// <summary>
		/// Gets the injector factory component.
		/// </summary>
		public IInjectorFactory InjectorFactory { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjectionStrategy"/> class.
		/// </summary>
		/// <param name="injectorFactory">The injector factory component.</param>
		public MethodInjectionStrategy(IInjectorFactory injectorFactory)
		{
			InjectorFactory = injectorFactory;
		}

		/// <summary>
		/// Injects values into the properties as described by <see cref="MethodInjectionDirective"/>s
		/// contained in the plan.
		/// </summary>
		/// <param name="context">The context.</param>
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