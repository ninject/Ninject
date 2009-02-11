using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure.Language;

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