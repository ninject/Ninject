// -------------------------------------------------------------------------------------------------
// <copyright file="IBinding.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Bindings
{
    using System;

    /// <summary>
    /// Contains information about a service registration.
    /// </summary>
    public interface IBinding : IBindingConfiguration
    {
        /// <summary>
        /// Gets the binding configuration.
        /// </summary>
        /// <value>The binding configuration.</value>
        IBindingConfiguration BindingConfiguration { get; }

        /// <summary>
        /// Gets the service type that is controlled by the binding.
        /// </summary>
        Type Service { get; }
    }
}