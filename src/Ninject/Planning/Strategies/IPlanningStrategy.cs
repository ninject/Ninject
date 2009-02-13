using System;
using Ninject.Components;

namespace Ninject.Planning.Strategies
{
	/// <summary>
	/// Contributes to the generation of a <see cref="IPlan"/>.
	/// </summary>
	public interface IPlanningStrategy : INinjectComponent
	{
		/// <summary>
		/// Contributes to the specified plan.
		/// </summary>
		/// <param name="plan">The plan that is being generated.</param>
		void Execute(IPlan plan);
	}
}