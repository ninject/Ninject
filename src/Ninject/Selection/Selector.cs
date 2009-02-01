using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Selection.Heuristics;

namespace Ninject.Selection
{
	public class Selector : ISelector
	{
		public BindingFlags Flags { get; set; }
		public IConstructorScorer ConstructorScorer { get; set; }
		public ICollection<IPropertyInjectionHeuristic> PropertyInjectionHeuristics { get; private set; }
		public ICollection<IMethodInjectionHeuristic> MethodInjectionHeuristics { get; private set; }
		public ICollection<IMethodInterceptionHeuristic> MethodInterceptionHeuristics { get; private set; }

		public Selector(IConstructorScorer constructorScorer, IEnumerable<IPropertyInjectionHeuristic> propertyInjectionHeuristics,
			IEnumerable<IMethodInjectionHeuristic> methodInjectionHeuristics, IEnumerable<IMethodInterceptionHeuristic> methodInterceptionHeuristics)
		{
			Flags = BindingFlags.Public | BindingFlags.Instance;
			ConstructorScorer = constructorScorer;
			PropertyInjectionHeuristics = propertyInjectionHeuristics.ToList();
			MethodInjectionHeuristics = methodInjectionHeuristics.ToList();
			MethodInterceptionHeuristics = methodInterceptionHeuristics.ToList();
		}

		public ConstructorInfo SelectConstructor(Type type)
		{
			ConstructorInfo constructor = type.GetConstructors(Flags).OrderByDescending(c => ConstructorScorer.Score(c)).FirstOrDefault();

			if (constructor == null)
				constructor = type.GetConstructor(Type.EmptyTypes);

			return constructor;
		}

		public IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type)
		{
			return type.GetProperties(Flags).Where(p => PropertyInjectionHeuristics.Any(h => h.ShouldInject(p)));
		}

		public IEnumerable<MethodInfo> SelectMethodsForInjection(Type type)
		{
			return type.GetMethods(Flags).Where(m => MethodInjectionHeuristics.Any(h => h.ShouldInject(m)));
		}

		public IEnumerable<MethodInfo> SelectMethodsForInterception(Type type)
		{
			const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			return type.GetMethods(flags).Where(m =>
				m.DeclaringType != typeof(object) && !m.IsPrivate && !m.IsFinal &&
				MethodInterceptionHeuristics.Any(h => h.ShouldIntercept(m)));
		}
	}
}