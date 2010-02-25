#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
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
	public abstract class DisposableObject : IDisposableObject
	{
		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

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