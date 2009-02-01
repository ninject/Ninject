using System;
using System.Reflection;
using Ninject.Infrastructure.Components;

namespace Ninject.Selection.Heuristics
{
	public interface IMethodInterceptionHeuristic : INinjectComponent
	{
		bool ShouldIntercept(MethodInfo method);
	}
}