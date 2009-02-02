using System;
using Ninject.Components;

namespace Ninject.Messaging
{
	public interface IBus : IDisposable, INinjectComponent
	{
		IChannel GetOrOpenChannel(string name);
		void CloseChannel(string name);
		void EnableChannel(string name);
		void DisableCannel(string name);
	}
}