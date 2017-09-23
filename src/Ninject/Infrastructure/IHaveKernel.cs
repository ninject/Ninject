// -------------------------------------------------------------------------------------------------
// <copyright file="IHaveKernel.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure
{
    /// <summary>
    /// Indicates that the object has a reference to an <see cref="IKernel"/>.
    /// </summary>
    public interface IHaveKernel
    {
        /// <summary>
        /// Gets the kernel.
        /// </summary>
        IKernel Kernel { get; }
    }
}