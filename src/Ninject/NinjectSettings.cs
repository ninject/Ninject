using System;

namespace Ninject
{
	/// <summary>
	/// Contains configuration options for Ninject.
	/// </summary>
	public class NinjectSettings : INinjectSettings
	{
		/// <summary>
		/// Gets or sets the attribute that indicates that a member should be injected.
		/// </summary>
		public Type InjectAttribute { get; set; }

		/// <summary>
		/// Gets or sets the cache prune timeout, in milliseconds.
		/// </summary>
		public int CachePruneTimeoutMs { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="NinjectSettings"/> class with the default values.
		/// </summary>
		public NinjectSettings()
		{
			InjectAttribute = typeof(InjectAttribute);
			CachePruneTimeoutMs = 1000;
		}
	}
}
