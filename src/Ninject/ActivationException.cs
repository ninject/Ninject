#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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
	#if !NO_EXCEPTION_SERIALIZATION
	[Serializable]
	#endif
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

		#if !NO_EXCEPTION_SERIALIZATION
		/// <summary>
		/// Initializes a new instance of the <see cref="ActivationException"/> class.
		/// </summary>
		/// <param name="info">The serialized object data.</param>
		/// <param name="context">The serialization context.</param>
		protected ActivationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		#endif
	}
}