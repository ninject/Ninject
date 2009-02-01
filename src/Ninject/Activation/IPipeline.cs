using System;
using System.Collections.Generic;
using Ninject.Activation.Strategies;
using Ninject.Infrastructure.Components;

namespace Ninject.Activation
{
	public interface IPipeline : INinjectComponent
	{
		IList<IActivationStrategy> Strategies { get; }
		void Activate(IContext context);
		void Deactivate(IContext context);
	}
}