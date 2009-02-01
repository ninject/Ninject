using System;
using System.Reflection;
using Ninject.Syntax;

namespace Ninject.Selection.Heuristics
{
	public class StandardMethodInjectionHeuristic : IMethodInjectionHeuristic
	{
		public bool ShouldInject(MethodInfo method)
		{
			return method.HasAttribute<InjectAttribute>();
		}
	}
}