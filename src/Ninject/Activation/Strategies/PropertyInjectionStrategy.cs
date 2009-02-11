using System;
using System.Linq;
using Ninject.Injection;
using Ninject.Parameters;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

namespace Ninject.Activation.Strategies
{
	public class PropertyInjectionStrategy : ActivationStrategy
	{
		public IInjectorFactory InjectorFactory { get; private set; }

		public PropertyInjectionStrategy(IInjectorFactory injectorFactory)
		{
			InjectorFactory = injectorFactory;
		}

		public override void Activate(IContext context)
		{
			foreach (var directive in context.Plan.GetAll<PropertyInjectionDirective>())
			{
				var injector = InjectorFactory.GetPropertyInjector(directive.Member);
				injector.Invoke(context.Instance, GetValue(context, directive.Target));
			}
		}

		public object GetValue(IContext context, ITarget target)
		{
			var parameter = context.Parameters.OfType<PropertyValue>().Where(p => p.Name == target.Name).SingleOrDefault();
			return parameter != null ? parameter.GetValue(context) : target.ResolveWithin(context);
		}
	}
}