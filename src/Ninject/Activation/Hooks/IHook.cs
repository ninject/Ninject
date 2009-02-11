using System;

namespace Ninject.Activation.Hooks
{
	public interface IHook
	{
		object Resolve();
	}
}