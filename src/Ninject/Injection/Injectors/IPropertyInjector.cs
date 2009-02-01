using System;
using System.Reflection;

namespace Ninject.Injection.Injectors
{
	public interface IPropertyInjector : IInjector<PropertyInfo>
	{
		void Invoke(object target, object value);
	}
}