using System;
using System.Reflection;

namespace Ninject.Planning.Directives
{
	public class MethodInjectionDirective : MultipleInjectionDirective<MethodInfo>
	{
		public MethodInjectionDirective(MethodInfo member) : base(member) { }
	}
}