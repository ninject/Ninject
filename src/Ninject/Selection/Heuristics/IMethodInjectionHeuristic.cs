using System;
using System.Reflection;
using Ninject.Infrastructure.Components;

namespace Ninject.Selection.Heuristics
{
	public interface IMethodInjectionHeuristic : INinjectComponent
	{
		bool ShouldInject(MethodInfo method);
	}
}