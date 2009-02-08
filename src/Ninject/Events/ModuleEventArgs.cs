using System;
using Ninject.Modules;

namespace Ninject.Events
{
	public class ModuleEventArgs : EventArgs
	{
		public IModule Module { get; private set; }

		public ModuleEventArgs(IModule module)
		{
			Module = module;
		}
	}
}