using System;

namespace Ninject.Infrastructure.Disposal
{
	/// <summary>
	/// An object that fires an event when it is disposed.
	/// </summary>
	public interface INotifyWhenDisposed : IDisposable
	{
		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		bool IsDisposed { get; }

		/// <summary>
		/// Occurs when the object is disposed.
		/// </summary>
		event EventHandler Disposed;
	}
}