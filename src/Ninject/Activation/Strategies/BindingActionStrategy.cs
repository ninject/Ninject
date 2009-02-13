using System;
using Ninject.Infrastructure.Language;

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// Executes actions defined on the binding during activation and deactivation.
	/// </summary>
	public class BindingActionStrategy : ActivationStrategy
	{
		/// <summary>
		/// Calls the activation actions defined on the binding.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Activate(IContext context)
		{
			context.Binding.ActivationActions.Map(action => action(context));
		}

		/// <summary>
		/// Calls the deactivation actions defined on the binding.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Deactivate(IContext context)
		{
			context.Binding.DeactivationActions.Map(action => action(context));
		}
	}
}