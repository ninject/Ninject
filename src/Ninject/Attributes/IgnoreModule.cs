using System;

namespace Ninject
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class IgnoreModule : Attribute { }
}
