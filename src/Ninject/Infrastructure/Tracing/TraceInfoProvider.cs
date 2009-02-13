using System;

namespace Ninject.Infrastructure.Tracing
{
	/// <summary>
	/// Creates a <see cref="TraceInfo"/> indicating the creation point of the object.
	/// </summary>
	public abstract class TraceInfoProvider : IHaveTraceInfo
	{
		/// <summary>
		/// Gets the trace info for the object.
		/// </summary>
		public TraceInfo TraceInfo { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TraceInfoProvider"/> class.
		/// </summary>
		protected TraceInfoProvider()
		{
#if DEBUG
			TraceInfo = TraceInfo.FromStackTrace();
#endif
		}
	}
}