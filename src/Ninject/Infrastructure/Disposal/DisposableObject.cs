#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using Ninject.Infrastructure.Language;
#endregion

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
		/// Occurs when the object is disposed.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Releases resources held by the object.
		/// </summary>
		public virtual void Dispose(bool disposing)
		{
			lock (this)
			{
				if (disposing && !IsDisposed)
				{
					var evt = Disposed;
					if (evt != null) evt(this, EventArgs.Empty);
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
			Dispose(false);
		}
	}
}