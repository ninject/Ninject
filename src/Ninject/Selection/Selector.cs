using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Components;
using Ninject.Selection.Heuristics;

namespace Ninject.Selection
{
	/// <summary>
	/// Selects members for injection.
	/// </summary>
	public class Selector : NinjectComponent, ISelector
	{
		private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance;

		/// <summary>
		/// Gets or sets the constructor scorer.
		/// </summary>
		public IConstructorScorer ConstructorScorer { get; set; }

		/// <summary>
		/// Gets the property injection heuristics.
		/// </summary>
		public ICollection<IPropertyInjectionHeuristic> PropertyInjectionHeuristics { get; private set; }

		/// <summary>
		/// Gets the method injection heuristics.
		/// </summary>
		public ICollection<IMethodInjectionHeuristic> MethodInjectionHeuristics { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Selector"/> class.
		/// </summary>
		/// <param name="constructorScorer">The constructor scorer.</param>
		/// <param name="propertyInjectionHeuristics">The property injection heuristics.</param>
		/// <param name="methodInjectionHeuristics">The method injection heuristics.</param>
		public Selector(IConstructorScorer constructorScorer, IEnumerable<IPropertyInjectionHeuristic> propertyInjectionHeuristics,
			IEnumerable<IMethodInjectionHeuristic> methodInjectionHeuristics)
		{
			ConstructorScorer = constructorScorer;
			PropertyInjectionHeuristics = propertyInjectionHeuristics.ToList();
			MethodInjectionHeuristics = methodInjectionHeuristics.ToList();
		}

		/// <summary>
		/// Selects the constructor to call on the specified type, by using the constructor scorer.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The selected constructor, or <see langword="null"/> if none were available.</returns>
		public ConstructorInfo SelectConstructor(Type type)
		{
			ConstructorInfo constructor = type.GetConstructors(Flags).OrderByDescending(c => ConstructorScorer.Score(c)).FirstOrDefault();

			if (constructor == null)
				constructor = type.GetConstructor(Type.EmptyTypes);

			return constructor;
		}

		/// <summary>
		/// Selects properties that should be injected, by using the property injection heuristics.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>A series of the selected properties.</returns>
		public IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type)
		{
			return type.GetProperties(Flags).Where(p => PropertyInjectionHeuristics.Any(h => h.ShouldInject(p)));
		}

		/// <summary>
		/// Selects methods that should be injected, by using the method injection heuristics.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>A series of the selected methods.</returns>
		public IEnumerable<MethodInfo> SelectMethodsForInjection(Type type)
		{
			return type.GetMethods(Flags).Where(m => MethodInjectionHeuristics.Any(h => h.ShouldInject(m)));
		}
	}
}