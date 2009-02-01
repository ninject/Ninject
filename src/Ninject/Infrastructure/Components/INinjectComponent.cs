using System;

namespace Ninject.Infrastructure.Components
{
	public interface INinjectComponent
	{
		INinjectSettings Settings { get; set; }
	}
}