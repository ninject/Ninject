using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure.Language;

namespace Ninject.Selection.Heuristics
{
	/// <summary>
	/// Scores constructors by either looking for the existence of an injection marker
	/// attribute, or by counting the number of parameters.
	/// </summary>
	public class StandardConstructorScorer : NinjectComponent, IConstructorScorer
	{
		/// <summary>
		/// Gets the score for the specified constructor.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <returns>The constructor's score.</returns>
		public int Score(ConstructorInfo constructor)
		{
			return constructor.HasAttribute(Kernel.Settings.InjectAttribute) ? Int32.MaxValue : constructor.GetParameters().Length;
		}
	}
}