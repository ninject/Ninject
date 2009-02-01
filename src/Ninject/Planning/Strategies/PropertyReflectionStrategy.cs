using System;
using System.Reflection;
using Ninject.Planning.Directives;
using Ninject.Selection;

namespace Ninject.Planning.Strategies
{
	public class PropertyReflectionStrategy : IPlanningStrategy
	{
		public ISelector Selector { get; set; }

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