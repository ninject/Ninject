using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure.Language;

namespace Ninject.Selection.Heuristics
{
	/// <summary>
	/// Determines whether methods should be injected during activation by checking
	/// if they are decorated with an injection marker attribute.
	/// </summary>
	public class StandardMethodInjectionHeuristic : NinjectComponent, IMethodInjectionHeuristic
	{
		/// <summary>
		/// Returns a value indicating whether the specified method should be injected.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns><c>True</c> if the method should be injected; otherwise <c>false</c>.</returns>
		public bool ShouldInject(MethodInfo method)
		{
			return method.HasAttribute(Kernel.Settings.InjectAttribute);
		}
	}
}