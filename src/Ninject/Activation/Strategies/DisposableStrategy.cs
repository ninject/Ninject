using System;

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// During deactivation, disposes instances that implement <see cref="IDisposable"/>.
	/// </summary>
	public class DisposableStrategy : ActivationStrategyFor<IDisposable>
	{
		/// <summary>
		/// Disposes the specified instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public override void Deactivate(IContext context, IDisposable instance)
		{
			instance.Dispose();
		}
	}
}