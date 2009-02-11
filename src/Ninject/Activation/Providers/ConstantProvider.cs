using System;

namespace Ninject.Activation.Providers
{
	public class ConstantProvider<T> : Provider<T>
	{
		public T Value { get; private set; }

		public ConstantProvider(T value)
		{
			Value = value;
		}

		protected override T CreateInstance(IContext context)
		{
			return Value;
		}
	}
}