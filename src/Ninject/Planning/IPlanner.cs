using System;
using System.Collections.Generic;
using Ninject.Components;
using Ninject.Planning.Strategies;

namespace Ninject.Planning
{
	/// <summary>
	/// Generates plans for how to activate instances.
	/// </summary>
	public interface IPlanner : INinjectComponent
	{
		/// <summary>
		/// Gets the strategies that contribute to the planning process.
		/// </summary>
		IList<IPlanningStrategy> Strategies { get; }

		/// <summary>
		/// Gets or creates an activation plan for the specified type.
		/// </summary>
		/// <param name="type">The type for which a plan should be created.</param>
		/// <returns>The type's activation plan.</returns>
		IPlan GetPlan(Type type);
	}
}