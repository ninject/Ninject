using System;
using System.Reflection;
using Ninject.Syntax;

namespace Ninject.Selection.Heuristics
{
	public class StandardPropertyInjectionHeuristic : IPropertyInjectionHeuristic
	{
		public bool ShouldInject(PropertyInfo property)
		{
			return property.HasAttribute<InjectAttribute>();
		}
	}
}