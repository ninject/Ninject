using System;
using Ninject.Planning.Bindings;

namespace Ninject.Events
{
	public class BindingEventArgs : EventArgs
	{
		public IBinding Binding { get; private set; }

		public BindingEventArgs(IBinding binding)
		{
			Binding = binding;
		}
	}
}