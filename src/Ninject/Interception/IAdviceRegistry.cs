using System;
using System.Collections.Generic;
using Ninject.Infrastructure.Components;
using Ninject.Interception.Advice;

namespace Ninject.Interception
{
	public interface IAdviceRegistry : INinjectComponent
	{
		void Register(IAdvice advice);
		bool HasAdvice(Type type);
		IEnumerable<IInterceptor> GetInterceptors(MethodCall methodCall);
	}
}