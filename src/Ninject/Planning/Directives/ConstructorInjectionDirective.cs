using System;
using System.Reflection;

namespace Ninject.Planning.Directives
{
	public class ConstructorInjectionDirective : MultipleInjectionDirective<ConstructorInfo>
	{
		public ConstructorInjectionDirective(ConstructorInfo member) : base(member) { }
	}
}