using System;

namespace Ninject.Infrastructure.Tracing
{
	public interface IHaveTraceInfo
	{
		TraceInfo TraceInfo { get; set; }
	}
}