using System;
using Ninject.Infrastructure.Language;

namespace Ninject.Infrastructure.Disposal
{
	/// <summary>
	/// An object that notifies when it is disposed.
	/// </summary>
	public abstract class DisposableObject : INotifyWhenDisposed
	{
		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Releases resources held by the object.
		/// </summary>
		public virtual void Dispose()
		{
			lock (this)
			{
				if (!IsDisposed)
				{
					Disposed.Raise(this, EventArgs.Empty);
					Disposed = null;
					IsDisposed = true;
					GC.SuppressFinalize(this);
				}
			}
		}

		/// <summary>
		/// Releases resources before the object is reclaimed by garbage collection.
		/// </summary>
		~DisposableObject()
		{
			Dispose();
		}

		/// <summary>
		/// Occurs when the object is disposed.
		/// </summary>
		public event EventHandler Disposed;
	}
}