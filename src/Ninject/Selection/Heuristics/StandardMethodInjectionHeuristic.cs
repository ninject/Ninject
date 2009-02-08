using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Syntax;

namespace Ninject.Selection.Heuristics
{
	public class StandardMethodInjectionHeuristic : NinjectComponent, IMethodInjectionHeuristic
	{
		public bool ShouldInject(MethodInfo method)
		{
			return method.HasAttribute(Kernel.Settings.InjectAttribute);
		}
	}
}