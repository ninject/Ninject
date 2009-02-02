using System;

namespace Ninject.Infrastructure.Tracing
{
	public abstract class TraceInfoProvider : IHaveTraceInfo
	{
		public TraceInfo TraceInfo { get; set; }

		protected TraceInfoProvider()
		{
#if DEBUG
			TraceInfo = TraceInfo.FromStackTrace();
#endif
		}
	}
}