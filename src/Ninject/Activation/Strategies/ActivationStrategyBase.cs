using System;
using Ninject.Infrastructure.Components;

namespace Ninject.Activation.Strategies
{
	public abstract class ActivationStrategyBase : NinjectComponent, IActivationStrategy
	{
		public virtual void Activate(IContext context) { }
		public virtual void Deactivate(IContext context) { }
	}
}