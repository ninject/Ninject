using System;
using Ninject.Components;

namespace Ninject.Planning.Strategies
{
	public interface IPlanningStrategy : INinjectComponent
	{
		void Execute(IPlan plan);
	}
}