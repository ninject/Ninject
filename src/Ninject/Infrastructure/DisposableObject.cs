using System;

namespace Ninject.Infrastructure
{
	public abstract class DisposableObject : INotifyWhenDisposed
	{
		public bool IsDisposed { get; private set; }

		public virtual void Dispose()
		{
			if (!IsDisposed)
			{
				Disposed(this, EventArgs.Empty);
				Disposed = null;
				IsDisposed = true;
				GC.SuppressFinalize(this);
			}
		}

		~DisposableObject()
		{
			Dispose();
		}

		public event EventHandler Disposed = delegate { };
	}
}