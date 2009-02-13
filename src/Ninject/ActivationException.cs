using System;
#if !NO_EXCEPTION_SERIALIZATION
using System.Runtime.Serialization;
#endif

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