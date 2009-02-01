using System;
using System.Reflection;

namespace Ninject.Injection.Injectors
{
	public interface IMethodInjector : IInjector<MethodInfo>
	{
		object Invoke(object target, params object[] values);
	}
}