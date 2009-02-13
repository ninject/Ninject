using System;

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// An activation strategy that is only executed if the instance being activated
	/// or deactivated is of a specified type.
	/// </summary>
	/// <typeparam name="T">The type of instance the strategy deals with.</typeparam>
	public abstract class ActivationStrategyFor<T> : ActivationStrategy
	{
		/// <summary>
		/// Contributes to the activation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public sealed override void Activate(IContext context)
		{
			if (context.Instance is T)
				Activate(context, (T)context.Instance);
		}

		/// <summary>
		/// Contributes to the deactivation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public sealed override void Deactivate(IContext context)
		{
			if (context.Instance is T)
				Deactivate(context, (T)context.Instance);
		}

		/// <summary>
		/// Contributes to the activation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public virtual void Activate(IContext context, T instance) { }

		/// <summary>
		/// Contributes to the deactivation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public virtual void Deactivate(IContext context, T instance) { }
	}
}