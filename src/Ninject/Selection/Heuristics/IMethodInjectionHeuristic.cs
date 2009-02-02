using System;
using System.Reflection;
using Ninject.Components;

namespace Ninject.Selection.Heuristics
{
	public interface IMethodInjectionHeuristic : INinjectComponent
	{
		bool ShouldInject(MethodInfo method);
	}
}