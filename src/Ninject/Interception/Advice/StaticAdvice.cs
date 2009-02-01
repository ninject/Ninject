using System;
using System.Reflection;

namespace Ninject.Interception.Advice
{
	public class StaticAdvice : IAdvice
	{
		public MethodInfo Method { get; set; }
		public Func<MethodCall, IInterceptor> InterceptorCallback { get; set; }

		public StaticAdvice(MethodInfo method, Func<MethodCall, IInterceptor> interceptorCallback)
		{
			Method = method;
			InterceptorCallback = interceptorCallback;
		}

		public bool Matches(Type type)
		{
			return Method.DeclaringType.IsAssignableFrom(type);
		}

		public bool Matches(MethodCall methodCall)
		{
			return methodCall.TargetMethod.Equals(Method);
		}

		public IInterceptor GetInterceptor(MethodCall methodCall)
		{
			return InterceptorCallback(methodCall);
		}
	}
}