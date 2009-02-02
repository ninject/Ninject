using System;

namespace Ninject.Infrastructure.Disposal
{
	public interface INotifyWhenDisposed : IDisposable
	{
		bool IsDisposed { get; }
		event EventHandler Disposed;
	}
}