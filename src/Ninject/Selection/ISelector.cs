using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Components;
using Ninject.Selection.Heuristics;

namespace Ninject.Selection
{
	/// <summary>
	/// Selects members for injection.
	/// </summary>
	public interface ISelector : INinjectComponent
	{
		/// <summary>
		/// Gets or sets the constructor scorer.
		/// </summary>
		IConstructorScorer ConstructorScorer { get; set; }

		/// <summary>
		/// Gets the property injection heuristics.
		/// </summary>
		ICollection<IPropertyInjectionHeuristic> PropertyInjectionHeuristics { get; }

		/// <summary>
		/// Gets the method injection heuristics.
		/// </summary>
		ICollection<IMethodInjectionHeuristic> MethodInjectionHeuristics { get; }

		/// <summary>
		/// Selects the constructor to call on the specified type, by using the constructor scorer.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The selected constructor, or <see langword="null"/> if none were available.</returns>
		ConstructorInfo SelectConstructor(Type type);

		/// <summary>
		/// Selects properties that should be injected, by using the property injection heuristics.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>A series of the selected properties.</returns>
		IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type);

		/// <summary>
		/// Selects methods that should be injected, by using the method injection heuristics.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>A series of the selected methods.</returns>
		IEnumerable<MethodInfo> SelectMethodsForInjection(Type type);
	}
}