using System;

namespace Ninject
{
	public interface INinjectSettings
	{
		Type InjectAttribute { get; }
		int CachePruneTimeoutMs { get; }
	}
}
