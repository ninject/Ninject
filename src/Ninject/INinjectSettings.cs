using System;

namespace Ninject
{
	/// <summary>
	/// Contains configuration options for Ninject.
	/// </summary>
	public interface INinjectSettings
	{
		/// <summary>
		/// Gets the attribute that indicates that a member should be injected.
		/// </summary>
		Type InjectAttribute { get; }

		/// <summary>
		/// Gets the cache prune timeout, in milliseconds.
		/// </summary>
		int CachePruneTimeoutMs { get; }
	}
}
