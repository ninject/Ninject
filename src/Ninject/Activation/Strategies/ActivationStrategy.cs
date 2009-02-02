using System;
using Ninject.Components;

namespace Ninject.Activation.Strategies
{
	public abstract class ActivationStrategy : NinjectComponent, IActivationStrategy
	{
		public virtual void Activate(IContext context) { }
		public virtual void Deactivate(IContext context) { }
	}
}