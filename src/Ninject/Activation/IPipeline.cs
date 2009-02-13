using System;
using System.Collections.Generic;
using Ninject.Activation.Strategies;
using Ninject.Components;

namespace Ninject.Activation
{
	/// <summary>
	/// Drives the activation (injection, etc.) of an instance.
	/// </summary>
	public interface IPipeline : INinjectComponent
	{
		/// <summary>
		/// Gets the strategies that contribute to the activation and deactivation processes.
		/// </summary>
		IList<IActivationStrategy> Strategies { get; }

		/// <summary>
		/// Activates the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		void Activate(IContext context);

		/// <summary>
		/// Deactivates the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		void Deactivate(IContext context);
	}
}