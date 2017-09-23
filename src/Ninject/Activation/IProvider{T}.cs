// -------------------------------------------------------------------------------------------------
// <copyright file="IProvider{T}.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Activation
{
    /// <summary>
    /// Provides instances ot the type T
    /// </summary>
    /// <typeparam name="T">The type provides by this implementation.</typeparam>
    public interface IProvider<T> : IProvider
    {
    }
}