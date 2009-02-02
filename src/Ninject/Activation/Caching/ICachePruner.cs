using System;
using Ninject.Components;

namespace Ninject.Activation.Caching
{
	public interface ICachePruner : INinjectComponent
	{
		void StartPruning(ICache cache);
		void StopPruning();
	}
}