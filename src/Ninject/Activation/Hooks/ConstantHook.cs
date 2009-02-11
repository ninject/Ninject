using System;

namespace Ninject.Activation.Hooks
{
	public class ConstantHook : IHook
	{
		public object Value { get; private set; }

		public ConstantHook(object value)
		{
			Value = value;
		}

		public object Resolve()
		{
			return Value;
		}
	}
}