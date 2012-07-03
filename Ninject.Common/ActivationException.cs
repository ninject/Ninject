#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#region Using Directives
using System;
#if !NO_EXCEPTION_SERIALIZATION
using System.Runtime.Serialization;
#endif
#endregion

namespace Ninject
{
    /// <summary>
    /// Indicates that an error occured during activation of an instance.
    /// </summary>

    public class ActivationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        public ActivationException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ActivationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ActivationException(string message, Exception innerException) : base(message, innerException) { }

    }
}