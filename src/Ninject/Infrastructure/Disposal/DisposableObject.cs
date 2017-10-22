// -------------------------------------------------------------------------------------------------
// <copyright file="DisposableObject.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure.Disposal
{
    using System;

    /// <summary>
    /// An object that notifies when it is disposed.
    /// </summary>
    public abstract class DisposableObject : IDisposableObject, INotifyWhenDisposed
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableObject"/> class.
        /// </summary>
        ~DisposableObject()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occurs when the object is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        /// <param name="disposing"><c>True</c> if called manually, otherwise by GC.</param>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !this.IsDisposed)
                {
                    this.IsDisposed = true;
                    this.Disposed?.Invoke(this, EventArgs.Empty);
                    this.Disposed = null;
                    GC.SuppressFinalize(this);
                }
            }
        }
    }
}