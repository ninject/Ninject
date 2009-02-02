using System;
using Ninject.Infrastructure.Disposal;

namespace Ninject.Components
{
	public abstract class NinjectComponent : DisposableObject, INinjectComponent
	{
		public INinjectSettings Settings { get; set; }
	}
}