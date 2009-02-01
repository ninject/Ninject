using System;

namespace Ninject.Interception.Advice
{
	public class DynamicAdvice : IAdvice
	{
		public Func<Type, bool> TypeMatcher { get; set; }
		public Func<MethodCall, bool> MethodCallMatcher { get; set; }
		public Func<MethodCall, IInterceptor> InterceptorCallback { get; set; }

		public DynamicAdvice(Func<Type, bool> typeMatcher, Func<MethodCall, bool> methodCallMatcher, Func<MethodCall, IInterceptor> interceptorCallback)
		{
			TypeMatcher = typeMatcher;
			MethodCallMatcher = methodCallMatcher;
			InterceptorCallback = interceptorCallback;
		}

		public bool Matches(Type type)
		{
			return TypeMatcher(type);
		}

		public bool Matches(MethodCall methodCall)
		{
			return MethodCallMatcher(methodCall);
		}

		public IInterceptor GetInterceptor(MethodCall methodCall)
		{
			return InterceptorCallback(methodCall);
		}
	}
}