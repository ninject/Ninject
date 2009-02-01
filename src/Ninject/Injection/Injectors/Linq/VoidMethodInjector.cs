using System;
using System.Reflection;

namespace Ninject.Injection.Injectors.Linq
{
	public class VoidMethodInjector : MethodInjectorBase<Action<object, object[]>>
	{
		public VoidMethodInjector(MethodInfo method) : base(method) { }

		public override object Invoke(object target, object[] values)
		{
			Callback.Invoke(target, values);
			return null;
		}
	}
}