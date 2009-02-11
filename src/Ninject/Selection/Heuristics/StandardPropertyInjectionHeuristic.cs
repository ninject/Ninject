using System;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure.Language;

namespace Ninject.Selection.Heuristics
{
	public class StandardPropertyInjectionHeuristic : NinjectComponent, IPropertyInjectionHeuristic
	{
		public bool ShouldInject(PropertyInfo property)
		{
			return property.HasAttribute(Kernel.Settings.InjectAttribute);
		}
	}
}