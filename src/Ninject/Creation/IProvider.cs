using System;
using Ninject.Activation;

namespace Ninject.Creation
{
	public interface IProvider
	{
		Type Prototype { get; }
		Type GetImplementationType(IContext context);
		object Create(IContext context);
	}
}