using System;
using Ninject.Planning.Bindings;

namespace Ninject.Events
{
	/// <summary>
	/// Data related to an event concerning an <see cref="IBinding"/>.
	/// </summary>
	public class BindingEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the binding associated with the event.
		/// </summary>
		public IBinding Binding { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BindingEventArgs"/> class.
		/// </summary>
		/// <param name="binding">The binding.</param>
		public BindingEventArgs(IBinding binding)
		{
			Binding = binding;
		}
	}
}