using System;

namespace Ninject.Activation.Strategies
{
	public class DisposableStrategy : ActivationStrategyFor<IDisposable>
	{
		public override void Deactivate(IContext context, IDisposable instance)
		{
			instance.Dispose();
		}
	}
}