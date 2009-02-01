using System;

namespace Ninject.Interception
{
	public interface IInterceptor
	{
		void Intercept(Invocation invocation);
	}
}