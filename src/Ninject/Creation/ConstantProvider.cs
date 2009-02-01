using System;
using Ninject.Activation;

namespace Ninject.Creation
{
	public class ConstantProvider<T> : Provider<T>
	{
		public T Value { get; set; }

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