// -------------------------------------------------------------------------------------------------
// <copyright file="INinjectComponent.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Components
{
    using System;

    /// <summary>
    /// A component that contributes to the internals of Ninject.
    /// </summary>
    public interface INinjectComponent : IDisposable
    {
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        INinjectSettings Settings { get; set; }
    }
}