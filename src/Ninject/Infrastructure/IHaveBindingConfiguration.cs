// -------------------------------------------------------------------------------------------------
// <copyright file="IHaveBindingConfiguration.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure
{
    using Ninject.Planning.Bindings;

    /// <summary>
    /// Indicates the object has a reference to a <see cref="IBinding"/>.
    /// </summary>
    public interface IHaveBindingConfiguration
    {
        /// <summary>
        /// Gets the binding.
        /// </summary>
        IBindingConfiguration BindingConfiguration { get; }
    }
}