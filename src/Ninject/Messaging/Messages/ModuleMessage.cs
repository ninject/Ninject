using System;
using Ninject.Modules;

namespace Ninject.Messaging.Messages
{
	public class ModuleMessage : EventArgs
	{
		public IModule Module { get; private set; }

		public ModuleMessage(IModule module)
		{
			Module = module;
		}
	}
}