using System;
using System.Reflection;
using Ninject.Injection.Injectors;

namespace Ninject.Messaging
{
	public interface IChannel : IDisposable
	{
		bool IsEnabled { get; set; }
		void AddPublication(object publisher, EventInfo evt);
		void AddSubscription(object subscriber, IMethodInjector injector, DeliveryThread thread);
		void RemoveAllPublications(object publisher);
		void RemoveAllSubscriptions(object subscriber);
	}
}