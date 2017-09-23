// -------------------------------------------------------------------------------------------------
// <copyright file="ICachePruner.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Caching
{
    using Ninject.Components;

    /// <summary>
    /// Prunes instances from an <see cref="ICache"/> based on environmental information.
    /// </summary>
    public interface ICachePruner : INinjectComponent
    {
        /// <summary>
        /// Starts pruning the specified cache based on the rules of the pruner.
        /// </summary>
        /// <param name="cache">The cache that will be pruned.</param>
        void Start(IPruneable cache);

        /// <summary>
        /// Stops pruning.
        /// </summary>
        void Stop();
    }
}