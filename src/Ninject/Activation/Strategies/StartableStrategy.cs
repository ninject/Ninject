using System;

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// Starts instances that implement <see cref="IStartable"/> during activation,
	/// and stops them during deactivation.
	/// </summary>
	public class StartableStrategy : ActivationStrategyFor<IStartable>
	{
		/// <summary>
		/// Starts the specified instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public override void Activate(IContext context, IStartable instance)
		{
			instance.Start();
		}

		/// <summary>
		/// Stops the specified instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public override void Deactivate(IContext context, IStartable instance)
		{
			instance.Stop();
		}
	}
}