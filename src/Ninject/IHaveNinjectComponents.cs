// -------------------------------------------------------------------------------------------------
// <copyright file="IHaveNinjectComponents.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    using Ninject.Components;

    /// <summary>
    /// Provides access to Ninject components.
    /// </summary>
    public interface IHaveNinjectComponents
    {
        /// <summary>
        /// Gets the component container, which holds components that contribute to Ninject.
        /// </summary>
        IComponentContainer Components { get; }
    }
}