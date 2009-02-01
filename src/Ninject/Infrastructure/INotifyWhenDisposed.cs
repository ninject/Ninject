using System;

namespace Ninject.Infrastructure
{
	public interface INotifyWhenDisposed : IDisposable
	{
		bool IsDisposed { get; }
		event EventHandler Disposed;
	}
}