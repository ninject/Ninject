using System;

namespace Ninject
{
	/// <summary>
	/// Indicates that the decorated member should be injected.
	/// </summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field,
		AllowMultiple = false, Inherited = true)]
	public class InjectAttribute : Attribute { }
}
