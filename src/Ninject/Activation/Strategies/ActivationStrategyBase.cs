using System;

namespace Ninject.Activation.Strategies
{
	public abstract class ActivationStrategyBase : IActivationStrategy
	{
		public virtual void Activate(IContext context) { }
		public virtual void Deactivate(IContext context) { }
	}
}