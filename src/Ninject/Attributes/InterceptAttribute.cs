using System;
using Ninject.Interception;

namespace Ninject
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public abstract class InterceptAttribute : Attribute
	{
		public int Order { get; set; }
		public abstract IInterceptor CreateInterceptor(MethodCall methodCall);
	}
}
