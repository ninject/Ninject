// -------------------------------------------------------------------------------------------------
// <copyright file="ActivationException.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Indicates that an error occurred during activation of an instance.
    /// </summary>
    [Serializable]
    public class ActivationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        public ActivationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ActivationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ActivationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationException"/> class.
        /// </summary>
        /// <param name="info">The serialized object data.</param>
        /// <param name="context">The serialization context.</param>
        protected ActivationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}