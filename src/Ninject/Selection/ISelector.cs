using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Infrastructure.Components;
using Ninject.Selection.Heuristics;

namespace Ninject.Selection
{
	public interface ISelector : INinjectComponent
	{
		BindingFlags Flags { get; set; }
		IConstructorScorer ConstructorScorer { get; set; }
		ICollection<IPropertyInjectionHeuristic> PropertyInjectionHeuristics { get; }
		ICollection<IMethodInjectionHeuristic> MethodInjectionHeuristics { get; }
		ICollection<IMethodInterceptionHeuristic> MethodInterceptionHeuristics { get; }

		ConstructorInfo SelectConstructor(Type type);
		IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type);
		IEnumerable<MethodInfo> SelectMethodsForInjection(Type type);
		IEnumerable<MethodInfo> SelectMethodsForInterception(Type type);
	}
}