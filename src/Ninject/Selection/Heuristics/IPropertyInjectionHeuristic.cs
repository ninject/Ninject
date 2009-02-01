using System;
using System.Reflection;
using Ninject.Infrastructure.Components;

namespace Ninject.Selection.Heuristics
{
	public interface IPropertyInjectionHeuristic : INinjectComponent
	{
		bool ShouldInject(PropertyInfo property);
	}
}