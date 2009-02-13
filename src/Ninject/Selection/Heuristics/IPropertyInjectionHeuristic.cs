using System;
using System.Reflection;
using Ninject.Components;

namespace Ninject.Selection.Heuristics
{
	/// <summary>
	/// Determines whether properties should be injected during activation.
	/// </summary>
	public interface IPropertyInjectionHeuristic : INinjectComponent
	{
		/// <summary>
		/// Returns a value indicating whether the specified property should be injected.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns><c>True</c> if the property should be injected; otherwise <c>false</c>.</returns>
		bool ShouldInject(PropertyInfo property);
	}
}