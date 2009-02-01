using System;

namespace Ninject.Infrastructure
{
	public interface INotifyWhenDisposed : IDisposable
	{
		event EventHandler Disposed;
	}
}