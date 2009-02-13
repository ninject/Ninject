using System;
using Ninject.Components;

namespace Ninject.Activation.Caching
{
	/// <summary>
	/// Periodically prunes an <see cref="ICache"/>.
	/// </summary>
	public interface ICachePruner : INinjectComponent
	{
		/// <summary>
		/// Starts periodically pruning the specified cache.
		/// </summary>
		/// <param name="cache">The cache to prune.</param>
		void StartPruning(ICache cache);

		/// <summary>
		/// Stops the periodic pruning operation.
		/// </summary>
		void StopPruning();
	}
}