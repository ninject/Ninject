using System;

namespace Ninject.Messaging
{
	public interface ISubscription
	{
		IChannel Channel { get; }
		object Subscriber { get; }
		bool IsAlive { get; }

		void Deliver(object sender, object args);
	}
}