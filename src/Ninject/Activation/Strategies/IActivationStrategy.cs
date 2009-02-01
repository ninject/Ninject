using System;
using Ninject.Infrastructure.Components;

namespace Ninject.Activation.Strategies
{
	public interface IActivationStrategy : INinjectComponent
	{
		void Activate(IContext context);
		void Deactivate(IContext context);
	}
}