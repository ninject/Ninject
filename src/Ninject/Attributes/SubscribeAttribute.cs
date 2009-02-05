using System;
using Ninject.Messaging;

namespace Ninject
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class SubscribeAttribute : Attribute
	{
		public string Channel { get; set; }
		public DeliveryThread DeliverOn { get; set; }

		public SubscribeAttribute(string channel)
		{
			Channel = channel;
		}
	}
}
