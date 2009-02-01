using System;

namespace Ninject.Activation.Strategies
{
	public class DisposableStrategy : ActivationStrategyBase
	{
		public override void Deactivate(IContext context)
		{
			var disposable = context.Instance as IDisposable;

			if (disposable != null)
				disposable.Dispose();
		}
	}
}