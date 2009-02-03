using System;

namespace Ninject.Activation.Providers
{
	public class CallbackProvider<T> : Provider<T>
	{
		public Func<IContext, T> Method { get; set; }

		public CallbackProvider(Func<IContext, T> method)
		{
			Method = method;
		}

		protected override T CreateInstance(IContext context)
		{
			return Method(context);
		}
	}
}