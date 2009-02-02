using System;

namespace Ninject
{
	public class NinjectSettings : INinjectSettings
	{
		public Type InjectAttribute { get; set; }
		public int CachePruneTimeoutMs { get; set; }

		public NinjectSettings()
		{
			InjectAttribute = typeof(InjectAttribute);
			CachePruneTimeoutMs = 1000;
		}
	}
}
