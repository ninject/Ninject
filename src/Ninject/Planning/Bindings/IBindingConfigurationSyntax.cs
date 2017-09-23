// -------------------------------------------------------------------------------------------------
// <copyright file="IBindingConfigurationSyntax.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Planning.Bindings
{
    using Ninject.Syntax;

    /// <summary>
    /// The syntax to define bindings.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    public interface IBindingConfigurationSyntax<T> :
        IBindingWhenInNamedWithOrOnSyntax<T>,
        IBindingInNamedWithOrOnSyntax<T>,
        IBindingNamedWithOrOnSyntax<T>,
        IBindingWithOrOnSyntax<T>
    {
    }
}