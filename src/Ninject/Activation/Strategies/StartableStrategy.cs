using System;

namespace Ninject.Activation.Strategies
{
	public class StartableStrategy : ActivationStrategyFor<IStartable>
	{
		public override void Activate(IContext context, IStartable instance)
		{
			instance.Start();
		}

		public override void Deactivate(IContext context, IStartable instance)
		{
			instance.Stop();
		}
	}
}