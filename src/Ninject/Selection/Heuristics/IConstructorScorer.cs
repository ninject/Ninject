using System;
using System.Reflection;
using Ninject.Components;

namespace Ninject.Selection.Heuristics
{
	/// <summary>
	/// Generates scores for constructors, to determine which is the best one to call during activation.
	/// </summary>
	public interface IConstructorScorer : INinjectComponent
	{
		/// <summary>
		/// Gets the score for the specified constructor.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <returns>The constructor's score.</returns>
		int Score(ConstructorInfo constructor);
	}
}