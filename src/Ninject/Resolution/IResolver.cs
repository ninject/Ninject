using System;
using System.Collections.Generic;
using Ninject.Activation;
using Ninject.Components;
using Ninject.Resolution.Strategies;

namespace Ninject.Resolution
{
	public interface IResolver : INinjectComponent
	{
		IList<IResolutionStrategy> Strategies { get; }
		bool HasStrategy(IRequest request);
		object Resolve(IContext context);
	}
}