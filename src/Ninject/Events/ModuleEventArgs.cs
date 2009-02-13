using System;
using Ninject.Modules;

namespace Ninject.Events
{
	/// <summary>
	/// Data related to an event concerning an <see cref="IModule"/>.
	/// </summary>
	public class ModuleEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the module associated with the event.
		/// </summary>
		public IModule Module { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleEventArgs"/> class.
		/// </summary>
		/// <param name="module">The module.</param>
		public ModuleEventArgs(IModule module)
		{
			Module = module;
		}
	}
}