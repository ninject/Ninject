using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Planning.Directives;
using Ninject.Selection;

namespace Ninject.Planning.Strategies
{
	public class MethodInterceptionStrategy : NinjectComponent, IPlanningStrategy
	{
		public ISelector Selector { get; private set; }

		public MethodInterceptionStrategy(ISelector selector)
		{
			Selector = selector;
		}

		public void Execute(IPlan plan)
		{
			foreach (MethodInfo method in Selector.SelectMethodsForInterception(plan.Type))
				plan.Add(new StaticMethodInterceptionDirective(method));
		}
	}
}