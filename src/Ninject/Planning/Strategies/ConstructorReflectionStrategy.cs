using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Planning.Directives;
using Ninject.Selection;

namespace Ninject.Planning.Strategies
{
	/// <summary>
	/// Adds a directive to plans indicating which constructor should be injected during activation.
	/// </summary>
	public class ConstructorReflectionStrategy : NinjectComponent, IPlanningStrategy
	{
		/// <summary>
		/// Gets the selector component.
		/// </summary>
		public ISelector Selector { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstructorReflectionStrategy"/> class.
		/// </summary>
		/// <param name="selector">The selector component.</param>
		public ConstructorReflectionStrategy(ISelector selector)
		{
			Selector = selector;
		}

		/// <summary>
		/// Adds a <see cref="ConstructorInjectionDirective"/> to the plan for the constructor
		/// that should be injected.
		/// </summary>
		/// <param name="plan">The plan that is being generated.</param>
		public void Execute(IPlan plan)
		{
			ConstructorInfo constructor = Selector.SelectConstructor(plan.Type);

			if (constructor != null)
				plan.Add(new ConstructorInjectionDirective(constructor));
		}
	}
}