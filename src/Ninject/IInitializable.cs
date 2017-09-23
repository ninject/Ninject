// -------------------------------------------------------------------------------------------------
// <copyright file="IInitializable.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    /// <summary>
    /// A service that requires initialization after it is activated.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Initializes the instance. Called during activation.
        /// </summary>
        void Initialize();
    }
}