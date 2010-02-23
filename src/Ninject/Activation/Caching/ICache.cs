#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
using Ninject.Components;
#endregion

namespace Ninject.Activation.Caching
{
	/// <summary>
	/// Tracks instances for re-use in certain scopes.
	/// </summary>
	public interface ICache : INinjectComponent
	{
		/// <summary>
		/// Gets the number of entries currently stored in the cache.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Stores the specified instance in the cache.
		/// </summary>
		/// <param name="context">The context to store.</param>
		/// <param name="reference">The instance reference.</param>
		void Remember(IContext context, InstanceReference reference);

		/// <summary>
		/// Tries to retrieve an instance to re-use in the specified context.
		/// </summary>
		/// <param name="context">The context that is being activated.</param>
		/// <returns>The instance for re-use, or <see langword="null"/> if none has been stored.</returns>
		object TryGet(IContext context);

		/// <summary>
		/// Deactivates and releases the specified instance from the cache.
		/// </summary>
		/// <param name="instance">The instance to release.</param>
		/// <returns><see langword="True"/> if the instance was found and released; otherwise <see langword="false"/>.</returns>
		bool Release(object instance);

		/// <summary>
		/// Removes instances from the cache which should no longer be re-used.
		/// </summary>
		void Prune();

		/// <summary>
		/// Immediately deactivates and removes all instances in the cache that are owned by
		/// the specified scope.
		/// </summary>
		/// <param name="scope">The scope whose instances should be deactivated.</param>
		void Clear(object scope);

		/// <summary>
		/// Immediately deactivates and removes all instances in the cache, regardless of scope.
		/// </summary>
		void Clear();
	}
}