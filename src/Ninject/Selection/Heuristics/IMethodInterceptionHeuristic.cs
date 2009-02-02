using System;
using System.Reflection;
using Ninject.Components;

namespace Ninject.Selection.Heuristics
{
	public interface IMethodInterceptionHeuristic : INinjectComponent
	{
		bool ShouldIntercept(MethodInfo method);
	}
}