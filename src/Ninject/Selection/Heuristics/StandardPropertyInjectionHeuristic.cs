using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure.Language;

namespace Ninject.Selection.Heuristics
{
	/// <summary>
	/// Determines whether properties should be injected during activation by checking
	/// if they are decorated with an injection marker attribute.
	/// </summary>
	public class StandardPropertyInjectionHeuristic : NinjectComponent, IPropertyInjectionHeuristic
	{
		/// <summary>
		/// Returns a value indicating whether the specified property should be injected.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns><c>True</c> if the property should be injected; otherwise <c>false</c>.</returns>
		public bool ShouldInject(PropertyInfo property)
		{
			return property.HasAttribute(Kernel.Settings.InjectAttribute);
		}
	}
}