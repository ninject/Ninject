using System;

namespace Ninject
{
	[AttributeUsage(AttributeTargets.Event, AllowMultiple = true, Inherited = true)]
	public class PublishAttribute : Attribute
	{
		public string Channel { get; set; }

		public PublishAttribute(string channel)
		{
			Channel = channel;
		}
	}
}
