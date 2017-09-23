// -------------------------------------------------------------------------------------------------
// <copyright file="INotifyWhenDisposed.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure.Disposal
{
    using System;

    /// <summary>
    /// An object that fires an event when it is disposed.
    /// </summary>
    public interface INotifyWhenDisposed : IDisposableObject
    {
        /// <summary>
        /// Occurs when the object is disposed.
        /// </summary>
        event EventHandler Disposed;
    }
}