// -------------------------------------------------------------------------------------------------
// <copyright file="IHaveNinjectSettings.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    /// <summary>
    /// Provides access to Ninject settings.
    /// </summary>
    public interface IHaveNinjectSettings
    {
        /// <summary>
        /// Gets the kernel settings.
        /// </summary>
        INinjectSettings Settings { get; }
    }
}