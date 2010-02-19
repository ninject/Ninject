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
	/// Prunes instances from an <see cref="ICache"/> based on environmental information.
	/// </summary>
	public interface ICachePruner : INinjectComponent
	{
		/// <summary>
		/// Starts pruning the specified cache based on the rules of the pruner.
		/// </summary>
		/// <param name="cache">The cache that will be pruned.</param>
		void Start(ICache cache);

		/// <summary>
		/// Stops pruning.
		/// </summary>
		void Stop();
	}
}