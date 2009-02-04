using System;

namespace Ninject.Activation
{
	public interface IProvider
	{
		Type Type { get; }
		object Create(IContext context);
	}
}