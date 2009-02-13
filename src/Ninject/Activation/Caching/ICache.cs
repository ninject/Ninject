using System;
using Ninject.Components;

namespace Ninject.Activation.Caching
{
	/// <summary>
	/// Tracks instances for re-use in certain scopes.
	/// </summary>
	public interface ICache : INinjectComponent
	{
		/// <summary>
		/// Stores the specified context in the cache.
		/// </summary>
		/// <param name="context">The context to store.</param>
		void Remember(IContext context);

		/// <summary>
		/// Tries to retrieve an instance to re-use in the specified context.
		/// </summary>
		/// <param name="context">The context that is being activated.</param>
		/// <returns>The instance for re-use, or <see langword="null"/> if none has been stored.</returns>
		object TryGet(IContext context);

		/// <summary>
		/// Removes instances from the cache whose scopes have been garbage collected.
		/// </summary>
		void Prune();
	}
}