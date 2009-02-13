using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Planning.Directives;
using Ninject.Selection;

namespace Ninject.Planning.Strategies
{
	/// <summary>
	/// Adds directives to plans indicating which methods should be injected during activation.
	/// </summary>
	public class MethodReflectionStrategy : NinjectComponent, IPlanningStrategy
	{
		/// <summary>
		/// Gets the selector component.
		/// </summary>
		public ISelector Selector { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodReflectionStrategy"/> class.
		/// </summary>
		/// <param name="selector">The selector component.</param>
		public MethodReflectionStrategy(ISelector selector)
		{
			Selector = selector;
		}

		/// <summary>
		/// Adds a <see cref="MethodInjectionDirective"/> to the plan for each method
		/// that should be injected.
		/// </summary>
		/// <param name="plan">The plan that is being generated.</param>
		public void Execute(IPlan plan)
		{
			foreach (MethodInfo method in Selector.SelectMethodsForInjection(plan.Type))
				plan.Add(new MethodInjectionDirective(method));
		}
	}
}