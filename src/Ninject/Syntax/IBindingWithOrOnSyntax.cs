// -------------------------------------------------------------------------------------------------
// <copyright file="IBindingWithOrOnSyntax.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Syntax
{
    /// <summary>
    /// Used to add additional information or actions to a binding.
    /// </summary>
    /// <typeparam name="T">The service being bound.</typeparam>
    public interface IBindingWithOrOnSyntax<T> : IBindingWithSyntax<T>, IBindingOnSyntax<T>
    {
    }
}