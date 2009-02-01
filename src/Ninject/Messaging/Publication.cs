using System;
using System.Reflection;

namespace Ninject.Messaging
{
	public class Publication : IPublication
	{
		private static readonly MethodInfo BroadcastMethod = typeof(Channel).GetMethod("Broadcast");

		public IChannel Channel { get; set; }
		public object Publisher { get; set; }
		public EventInfo Event { get; set; }
		public Delegate PublishDelegate { get; private set; }

		public Publication(IChannel channel, object publisher, EventInfo evt)
		{
			Channel = channel;
			Publisher = publisher;
			Event = evt;
			PublishDelegate = Delegate.CreateDelegate(Event.EventHandlerType, Channel, BroadcastMethod);
			Event.AddEventHandler(Publisher, PublishDelegate);
		}

		public void Dispose()
		{
			Event.RemoveEventHandler(Publisher, PublishDelegate);
			PublishDelegate = null;
			GC.SuppressFinalize(this);
		}
	}
}