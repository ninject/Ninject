// -------------------------------------------------------------------------------------------------
// <copyright file="IStartable.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    /// <summary>
    /// A service that is started when activated, and stopped when deactivated.
    /// </summary>
    public interface IStartable
    {
        /// <summary>
        /// Starts this instance. Called during activation.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this instance. Called during deactivation.
        /// </summary>
        void Stop();
    }
}