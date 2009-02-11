using System;
using System.Reflection;
using Ninject.Planning.Targets;

namespace Ninject.Planning.Directives
{
	public class PropertyInjectionDirective : IDirective
	{
		public PropertyInfo Member { get; private set; }
		public ITarget Target { get; private set; }

		public PropertyInjectionDirective(PropertyInfo member)
		{
			Member = member;
			Target = new PropertyTarget(member);
		}
	}
}