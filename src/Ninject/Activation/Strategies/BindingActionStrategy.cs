using System;
using Ninject.Infrastructure.Language;

namespace Ninject.Activation.Strategies
{
	public class BindingActionStrategy : ActivationStrategy
	{
		public override void Activate(IContext context)
		{
			context.Binding.ActivationActions.Map(action => action(context));
		}

		public override void Deactivate(IContext context)
		{
			context.Binding.DeactivationActions.Map(action => action(context));
		}
	}
}