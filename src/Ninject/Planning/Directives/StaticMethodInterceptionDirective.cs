using System;
using System.Reflection;

namespace Ninject.Planning.Directives
{
	public class StaticMethodInterceptionDirective : IDirective
	{
		public MethodInfo Method { get; set; }

		public StaticMethodInterceptionDirective(MethodInfo method)
		{
			Method = method;
		}
	}
}