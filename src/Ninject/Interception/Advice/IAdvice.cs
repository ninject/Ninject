using System;

namespace Ninject.Interception.Advice
{
	public interface IAdvice
	{
		bool Matches(Type type);
		bool Matches(MethodCall methodCall);
		IInterceptor GetInterceptor(MethodCall methodCall);
	}
}