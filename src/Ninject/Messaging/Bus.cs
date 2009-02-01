using System;
using System.Collections.Generic;
using Ninject.Infrastructure.Components;
using Ninject.Injection;

namespace Ninject.Messaging
{
	public class Bus : NinjectComponent, IBus
	{
		private readonly Dictionary<string, IChannel> _channels = new Dictionary<string, IChannel>();

		public IInjectorFactory InjectorFactory { get; set; }

		public Bus(IInjectorFactory injectorFactory)
		{
			InjectorFactory = injectorFactory;
		}

		public override void Dispose()
		{
			foreach (IChannel channel in _channels.Values)
				channel.Dispose();

			_channels.Clear();

			base.Dispose();
		}

		public IChannel GetOrOpenChannel(string name)
		{
			if (!_channels.ContainsKey(name))
				_channels.Add(name, CreateChannel(name));

			return _channels[name];
		}

		public void CloseChannel(string name)
		{
			if (_channels.ContainsKey(name))
			{
				_channels[name].Dispose();
				_channels.Remove(name);
			}
		}

		public void EnableChannel(string name)
		{
			GetOrOpenChannel(name).IsEnabled = true;
		}

		public void DisableCannel(string name)
		{
			GetOrOpenChannel(name).IsEnabled = false;
		}

		protected virtual IChannel CreateChannel(string name)
		{
			return new Channel(name, InjectorFactory);
		}
	}
}