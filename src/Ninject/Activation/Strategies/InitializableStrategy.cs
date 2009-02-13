using System;

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// During activation, initializes instances that implement <see cref="IInitializable"/>.
	/// </summary>
	public class InitializableStrategy : ActivationStrategyFor<IInitializable>
	{
		/// <summary>
		/// Initializes the specified instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public override void Activate(IContext context, IInitializable instance)
		{
			instance.Initialize();
		}
	}
}