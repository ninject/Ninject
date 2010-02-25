using System;
using Ninject.Infrastructure.Disposal;

namespace Ninject.Tests.Fakes
{
	public class NotifiesWhenDisposed : DisposableObject, INotifyWhenDisposed
	{
		public event EventHandler Disposed;

		public override void Dispose(bool disposing)
		{
			lock (this)
			{
				if (disposing && !IsDisposed)
				{
					var evt = Disposed;
					if (evt != null) evt(this, EventArgs.Empty);
					Disposed = null;
				}

				base.Dispose(disposing);
			}
		}
	}
}