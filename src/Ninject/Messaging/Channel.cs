using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject.Infrastructure.Disposal;
using Ninject.Injection.Injectors;
using Ninject.Syntax;

namespace Ninject.Messaging
{
	public class Channel : DisposableObject, IChannel
	{
		public string Name { get; private set; }
		public ICollection<IPublication> Publications { get; private set; }
		public ICollection<ISubscription> Subscriptions { get; private set; }
		public bool IsEnabled { get; set; }

		public Channel(string name)
		{
			Name = name;
			Publications = new List<IPublication>();
			Subscriptions = new List<ISubscription>();
		}

		public override void Dispose()
		{
			Publications.Map(publication => publication.Dispose());

			Publications.Clear();
			Subscriptions.Clear();

			base.Dispose();
		}

		public void AddPublication(object publisher, EventInfo evt)
		{
			Publications.Add(new Publication(this, publisher, evt));
		}

		public void AddSubscription(object subscriber, IMethodInjector injector, DeliveryThread thread)
		{
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