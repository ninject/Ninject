using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Planning.Directives;
using Ninject.Selection;

namespace Ninject.Planning.Strategies
{
	public class PropertyReflectionStrategy : NinjectComponent, IPlanningStrategy
	{
		public ISelector Selector { get; private set; }

		public PropertyReflectionStrategy(ISelector selector)
		{
			Selector = selector;
		}

		public void Execute(IPlan plan)
		{
			foreach (PropertyInfo property in Selector.SelectPropertiesForInjection(plan.Type))
				plan.Add(new PropertyInjectionDirective(property));
		}
	}
}