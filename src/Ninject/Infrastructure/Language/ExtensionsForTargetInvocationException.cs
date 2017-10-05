// -------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsForTargetInvocationException.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2017, Ninject Project Contributors
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Infrastructure.Language
{
    using System.Reflection;
    using System.Runtime.ExceptionServices;

    /// <summary>
    /// Provides extension methods for <see cref="TargetInvocationException"/>.
    /// </summary>
    internal static class ExtensionsForTargetInvocationException
    {
        /// <summary>
        /// Re-throws inner exception.
        /// </summary>
        /// <param name="exception">The <see cref="TargetInvocationException"/>.</param>
        public static void RethrowInnerException(this TargetInvocationException exception)
        {
            var innerException = exception.InnerException;
            ExceptionDispatchInfo.Capture(innerException).Throw();

            throw innerException;
        }
    }
}