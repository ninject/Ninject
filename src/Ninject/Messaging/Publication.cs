using System;
using System.Reflection;
using Ninject.Infrastructure.Disposal;

namespace Ninject.Messaging
{
	public class Publication : DisposableObject, IPublication
	{
		private static readonly MethodInfo BroadcastMethod = typeof(IChannel).GetMethod("Broadcast");

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

		public override void Dispose()
		{
			Event.RemoveEventHandler(Publisher, PublishDelegate);
			PublishDelegate = null;
			base.Dispose();
		}
	}
}