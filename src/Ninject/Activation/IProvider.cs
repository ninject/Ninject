using System;

namespace Ninject.Activation
{
	public interface IProvider
	{
		Type Prototype { get; }
		Type GetImplementationType(IContext context);
		object Create(IContext context);
	}
}