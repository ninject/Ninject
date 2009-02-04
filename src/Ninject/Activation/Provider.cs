using System;

namespace Ninject.Activation
{
	public abstract class Provider<T> : IProvider
	{
		public virtual Type Type
		{
			get { return typeof(T); }
		}

		public object Create(IContext context)
		{
			return CreateInstance(context);
		}

		protected abstract T CreateInstance(IContext context);
	}
}