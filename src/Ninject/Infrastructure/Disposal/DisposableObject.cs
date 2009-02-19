#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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
			Dispose(false);
		}

		/// <summary>
		/// Occurs when the object is disposed.
		/// </summary>
		public event EventHandler Disposed;
	}
}