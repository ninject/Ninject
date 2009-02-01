using System;

namespace Ninject.Injection.Injectors
{
	public interface IFieldInjector
	{
		void Invoke(object target, object value);
	}
}