using System;
using System.Reflection;

namespace Ninject.Injection.Injectors.Linq
{
	public class MethodInjector : MethodInjectorBase<Func<object, object[], object>>
	{
		public MethodInjector(MethodInfo method) : base(method) { }

		public override object Invoke(object target, object[] values)
		{
			return Callback.Invoke(target, values);
		}
	}
}