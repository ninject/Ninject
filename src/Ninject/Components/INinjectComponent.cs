using System;

namespace Ninject.Components
{
	public interface INinjectComponent : IDisposable
	{
		INinjectSettings Settings { get; set; }
	}
}