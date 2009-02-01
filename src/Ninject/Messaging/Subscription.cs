using System;
using System.Threading;
using Ninject.Injection.Injectors;

namespace Ninject.Messaging
{
	public class Subscription : ISubscription
	{
		private readonly WeakReference _subscriber;

		public IChannel Channel { get; set; }
		public IMethodInjector Injector { get; set; }
		public DeliveryThread Thread { get; set; }
		public SynchronizationContext SynchronizationContext { get; private set; }

		public object Subscriber
		{
			get { return _subscriber.Target; }
		}

		public bool IsAlive
		{
			get { return _subscriber.IsAlive; }
		}

		public Subscription(IChannel channel, object subscriber, IMethodInjector injector, DeliveryThread thread)
		{
			_subscriber = new WeakReference(subscriber);

			Channel = channel;
			Injector = injector;
			Thread = thread;

			if (thread == DeliveryThread.UserInterface)
				SynchronizationContext = SynchronizationContext.Current;
		}

		public void Deliver(object sender, object args)
		{
			switch (Thread)
			{
				case DeliveryThread.Current:
					DeliverMessage(sender, args);
					break;

				case DeliveryThread.Background:
					ThreadPool.QueueUserWorkItem(x => DeliverMessage(sender, args));
					break;

				case DeliveryThread.UserInterface:
					if (SynchronizationContext != null)
						SynchronizationContext.Send(x => DeliverMessage(sender, args), null);
					else
						DeliverMessage(sender, args);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void DeliverMessage(object sender, object args)
		{
			if (_subscriber.IsAlive)
				Injector.Invoke(_subscriber.Target, sender, args);
		}
	}
}