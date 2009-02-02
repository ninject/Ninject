using System;

namespace Ninject.Activation.Strategies
{
	public class InitializableStrategy : ActivationStrategyFor<IInitializable>
	{
		public override void Activate(IContext context, IInitializable instance)
		{
			instance.Initialize();
		}
	}
}