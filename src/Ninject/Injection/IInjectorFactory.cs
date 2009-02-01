using System;
using System.Reflection;
using Ninject.Infrastructure.Components;
using Ninject.Injection.Injectors;

namespace Ninject.Injection
{
	public interface IInjectorFactory : INinjectComponent
	{
		IConstructorInjector GetConstructorInjector(ConstructorInfo constructor);
		IPropertyInjector GetPropertyInjector(PropertyInfo property);
		IMethodInjector GetMethodInjector(MethodInfo method);
	}
}