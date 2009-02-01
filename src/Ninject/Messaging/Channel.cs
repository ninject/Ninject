using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Injection;
using Ninject.Syntax;

namespace Ninject.Messaging
{
	public class Channel : IChannel
	{
		public string Name { get; private set; }
		public IInjectorFactory InjectorFactory { get; set; }
		public ICollection<IPublication> Publications { get; private set; }
		public ICollection<ISubscription> Subscriptions { get; private set; }
		public bool IsEnabled { get; set; }

		public Channel(string name, IInjectorFactory injectorFactory)
		{
			Name = name;
			InjectorFactory = injectorFactory;
			Publications = new List<IPublication>();
			Subscriptions = new List<ISubscription>();
		}

		public void Dispose()
		{
			Publications.Map(publication => publication.Dispose());
			Publications.Clear();
			Subscriptions.Clear();
			GC.SuppressFinalize(this);
		}

		public void AddPublication(object publisher, EventInfo evt)
		{
			Publications.Add(new Publication(this, publisher, evt));
		}

		public void AddSubscription(object subscriber, MethodInfo handler, DeliveryThread thread)
		{
			var injector = InjectorFactory.GetMethodInjector(handler);
			Subscriptions.Add(new Subscription(this, subscriber, injector, thread));
		}

		public void RemoveAllPublications(object publisher)
		{
			lock (Publications)
			{
				foreach (var publication in Publications.Where(p => p.Publisher == publisher).ToArray())
				{
					publication.Dispose();
					Publications.Remove(publication);
				}
			}
		}

		public void RemoveAllSubscriptions(object subscriber)
		{
			lock (Subscriptions)
			{
				Subscriptions.RemoveWhere(s => s.Subscriber == subscriber);
			}
		}

		public void Broadcast(object publisher, object args)
		{
			PruneListeners();

			if (!IsEnabled)
				return;

			lock (Subscriptions)
			{
				Subscriptions.Map(subscription => subscription.Deliver(publisher, args));
			}
		}

		private void PruneListeners()
		{
			Subscriptions.RemoveWhere(subscription => !subscription.IsAlive);
		}
	}
}