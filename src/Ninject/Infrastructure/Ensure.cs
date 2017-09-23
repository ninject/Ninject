// -------------------------------------------------------------------------------------------------
// <copyright file="Ensure.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure
{
    using System;

    /// <summary>
    /// Argument guard.
    /// </summary>
    internal static class Ensure
    {
        /// <summary>
        /// Ensures the argument is not null.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        /// <param name="name">The argument name.</param>
        internal static void ArgumentNotNull(object argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name, "Cannot be null");
            }
        }

        /// <summary>
        /// Ensures the argument is not null or empty.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        /// <param name="name">The argument name.</param>
        internal static void ArgumentNotNullOrEmpty(string argument, string name)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException("Cannot be null or empty", name);
            }
        }
    }
}