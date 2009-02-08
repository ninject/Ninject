using System;

namespace Ninject.Activation
{
	public interface IHook
	{
		object Resolve();
	}
}