using System;

namespace Ninject.Activation.Strategies
{
	public abstract class ActivationStrategyFor<T> : ActivationStrategy
	{
		public sealed override void Activate(IContext context)
		{
			if (context.Instance is T)
				Activate(context, (T)context.Instance);
		}

		public sealed override void Deactivate(IContext context)
		{
			if (context.Instance is T)
				Deactivate(context, (T)context.Instance);
		}

		public virtual void Activate(IContext context, T instance) { }
		public virtual void Deactivate(IContext context, T instance) { }
	}
}