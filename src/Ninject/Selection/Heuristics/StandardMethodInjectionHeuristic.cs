using System;
using System.Reflection;
using Ninject.Infrastructure.Components;
using Ninject.Syntax;

namespace Ninject.Selection.Heuristics
{
	public class StandardMethodInjectionHeuristic : NinjectComponent, IMethodInjectionHeuristic
	{
		public bool ShouldInject(MethodInfo method)
		{
			return method.HasAttribute<InjectAttribute>();
		}
	}
}