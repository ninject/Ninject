using System;

namespace Ninject.Infrastructure.Language
{
	internal static class ExtensionsForEventHandler
	{
		public static void Raise<T>(this EventHandler<T> handler, object sender, T message)
			where T : EventArgs
		{
			EventHandler<T> evt = handler;
			if (evt != null) evt(sender, message);
		}
	}
}