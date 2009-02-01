using System;
using System.Reflection;

namespace Ninject.Injection.Injectors
{
	public interface IConstructorInjector : IInjector<ConstructorInfo>
	{
		object Invoke(params object[] values);
	}
}