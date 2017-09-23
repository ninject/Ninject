// -------------------------------------------------------------------------------------------------
// <copyright file="IPruneable.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation.Caching
{
    /// <summary>
    /// An object that is pruneable.
    /// </summary>
    public interface IPruneable
    {
        /// <summary>
        /// Removes instances from the cache which should no longer be re-used.
        /// </summary>
        void Prune();
    }
}