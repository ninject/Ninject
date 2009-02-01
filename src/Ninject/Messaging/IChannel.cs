using System;
using System.Reflection;

namespace Ninject.Messaging
{
	public interface IChannel : IDisposable
	{
		bool IsEnabled { get; set; }
		void AddPublication(object publisher, EventInfo evt);
		void AddSubscription(object subscriber, MethodInfo handler, DeliveryThread thread);
		void RemoveAllPublications(object publisher);
		void RemoveAllSubscriptions(object subscriber);
	}
}