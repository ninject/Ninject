using System;
using Ninject.Activation;
using Ninject.Components;

namespace Ninject.Resolution.Strategies
{
	public interface IResolutionStrategy : INinjectComponent
	{
		bool Supports(IRequest request);
		object Resolve(IContext context);
	}
}