using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure.Language;

namespace Ninject.Selection.Heuristics
{
	public class StandardMethodInterceptionHeuristic : NinjectComponent, IMethodInterceptionHeuristic
	{
		public bool ShouldIntercept(MethodInfo method)
		{
			return method.HasAttribute(Kernel.Settings.InjectAttribute);
		}
	}
}