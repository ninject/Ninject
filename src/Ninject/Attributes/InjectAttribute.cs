using System;

namespace Ninject
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field,
		AllowMultiple = false, Inherited = true)]
	public class InjectAttribute : Attribute { }
}
