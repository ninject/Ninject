using System;
using System.Linq;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// Injects properties on an instance during activation.
	/// </summary>
	public class PropertyInjectionStrategy : ActivationStrategy
	{
		/// <summary>
		/// Gets the injector factory component.
		/// </summary>
		public IInjectorFactory InjectorFactory { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyInjectionStrategy"/> class.
		/// </summary>
		/// <param name="injectorFactory">The injector factory component.</param>
		public PropertyInjectionStrategy(IInjectorFactory injectorFactory)
		{
			InjectorFactory = injectorFactory;
		}

		/// <summary>
		/// Injects values into the properties as described by <see cref="PropertyInjectionDirective"/>s
		/// contained in the plan.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Activate(IContext context)
		{
			foreach (var directive in context.Plan.GetAll<PropertyInjectionDirective>())
			{
				var injector = InjectorFactory.GetPropertyInjector(directive.Member);
				injector.Invoke(context.Instance, GetValue(context, directive.Target));
			}
		}

		/// <summary>
		/// Gets the value to inject into the specified target.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="target">The target.</param>
		/// <returns>The value to inject into the specified target.</returns>
		public object GetValue(IContext context, ITarget target)
		{
			var parameter = context.Parameters.OfType<PropertyValue>().Where(p => p.Name == target.Name).SingleOrDefault();
			return parameter != null ? parameter.GetValue(context) : target.ResolveWithin(context);
		}
	}
}