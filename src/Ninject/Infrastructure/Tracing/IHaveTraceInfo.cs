using System;

namespace Ninject.Infrastructure.Tracing
{
	/// <summary>
	/// Indicates that the object has <see cref="TraceInfo"/>.
	/// </summary>
	public interface IHaveTraceInfo
	{
		/// <summary>
		/// Gets the trace info for the object.
		/// </summary>
		TraceInfo TraceInfo { get; }
	}
}