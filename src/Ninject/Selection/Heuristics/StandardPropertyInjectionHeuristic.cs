using System;
using System.Reflection;
using Ninject.Infrastructure.Components;
using Ninject.Syntax;

namespace Ninject.Selection.Heuristics
{
	public class StandardPropertyInjectionHeuristic : NinjectComponent, IPropertyInjectionHeuristic
	{
		public bool ShouldInject(PropertyInfo property)
		{
			return property.HasAttribute<InjectAttribute>();
		}
	}
}