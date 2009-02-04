using System;
using Ninject.Activation;
using Ninject.Components;

namespace Ninject.Resolution
{
	public interface IResolver : INinjectComponent
	{
		object Resolve(IContext context);
	}
}