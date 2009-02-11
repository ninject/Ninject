using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Planning.Directives;
using Ninject.Selection;

namespace Ninject.Planning.Strategies
{
	public class MethodReflectionStrategy : NinjectComponent, IPlanningStrategy
	{
		public ISelector Selector { get; private set; }

		public MethodReflectionStrategy(ISelector selector)
		{
			Selector = selector;
		}

		public void Execute(IPlan plan)
		{
			foreach (MethodInfo method in Selector.SelectMethodsForInjection(plan.Type))
				plan.Add(new MethodInjectionDirective(method));
		}
	}
}