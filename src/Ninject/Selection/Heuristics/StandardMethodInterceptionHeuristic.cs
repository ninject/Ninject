using System;
using System.Reflection;
using Ninject.Syntax;

namespace Ninject.Selection.Heuristics
{
	public class StandardMethodInterceptionHeuristic : IMethodInterceptionHeuristic
	{
		public bool ShouldIntercept(MethodInfo method)
		{
			return method.HasAttribute<InterceptAttribute>();
		}
	}
}