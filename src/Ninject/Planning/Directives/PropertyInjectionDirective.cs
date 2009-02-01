using System;
using System.Reflection;
using Ninject.Planning.Targets;

namespace Ninject.Planning.Directives
{
	public class PropertyInjectionDirective : IDirective
	{
		public PropertyInfo Member { get; set; }
		public ITarget Target { get; set; }

		public PropertyInjectionDirective(PropertyInfo member)
		{
			Member = member;
			Target = new PropertyTarget(member);
		}
	}
}