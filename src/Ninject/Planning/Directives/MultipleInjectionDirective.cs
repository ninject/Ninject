using System;
using System.Linq;
using System.Reflection;
using Ninject.Planning.Targets;

namespace Ninject.Planning.Directives
{
	public abstract class MultipleInjectionDirective<T> : IDirective
		where T : MethodBase
	{
		public T Member { get; private set; }
		public ITarget[] Targets { get; private set; }

		protected MultipleInjectionDirective(T member)
		{
			Member = member;
			Targets = GetParameterTargets(member);
		}

		protected ITarget[] GetParameterTargets(T method)
		{
			return method.GetParameters().Select(parameter => new ParameterTarget(parameter)).ToArray();
		}
	}
}