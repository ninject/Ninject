using System;
using Ninject.Components;

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// Contributes to a <see cref="IPipeline"/>, and is called during the activation
	/// and deactivation of an instance.
	/// </summary>
	public interface IActivationStrategy : INinjectComponent
	{
		/// <summary>
		/// Contributes to the activation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		void Activate(IContext context);

		/// <summary>
		/// Contributes to the deactivation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		void Deactivate(IContext context);
	}
}