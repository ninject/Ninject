using System;
using Ninject.Infrastructure.Components;

namespace Ninject.Planning.Strategies
{
	public interface IPlanningStrategy : INinjectComponent
	{
		void Execute(IPlan plan);
	}
}