using System;
using Ninject.Planning.Bindings;

namespace Ninject.Messaging.Messages
{
	public class BindingMessage : EventArgs
	{
		public IBinding Binding { get; private set; }

		public BindingMessage(IBinding binding)
		{
			Binding = binding;
		}
	}
}