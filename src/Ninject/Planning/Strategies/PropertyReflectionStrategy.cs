using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Planning.Directives;
using Ninject.Selection;

namespace Ninject.Planning.Strategies
{
	/// <summary>
	/// Adds directives to plans indicating which properties should be injected during activation.
	/// </summary>
	public class PropertyReflectionStrategy : NinjectComponent, IPlanningStrategy
	{
		/// <summary>
		/// Gets the selector component.
		/// </summary>
		public ISelector Selector { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyReflectionStrategy"/> class.
		/// </summary>
		/// <param name="selector">The selector component.</param>
		public PropertyReflectionStrategy(ISelector selector)
		{
			Selector = selector;
		}

		/// <summary>
		/// Adds a <see cref="PropertyInjectionDirective"/> to the plan for each property
		/// that should be injected.
		/// </summary>
		/// <param name="plan">The plan that is being generated.</param>
		public void Execute(IPlan plan)
		{
			foreach (PropertyInfo property in Selector.SelectPropertiesForInjection(plan.Type))
				plan.Add(new PropertyInjectionDirective(property));
		}
	}
}