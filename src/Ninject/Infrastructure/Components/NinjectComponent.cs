using System;

namespace Ninject.Infrastructure.Components
{
	public abstract class NinjectComponent : DisposableObject, INinjectComponent
	{
		public INinjectSettings Settings { get; set; }
	}
}