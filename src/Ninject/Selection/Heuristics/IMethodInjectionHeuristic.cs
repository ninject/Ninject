using System;
using System.Reflection;
using Ninject.Components;

namespace Ninject.Selection.Heuristics
{
	/// <summary>
	/// Determines whether methods should be injected during activation.
	/// </summary>
	public interface IMethodInjectionHeuristic : INinjectComponent
	{
		/// <summary>
		/// Returns a value indicating whether the specified method should be injected.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns><c>True</c> if the method should be injected; otherwise <c>false</c>.</returns>
		bool ShouldInject(MethodInfo method);
	}
}