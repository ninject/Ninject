using System;
using LinFu.DynamicProxy;
using Ninject.Injection;

namespace Ninject.Interception
{
	public class Dispatcher : LinFu.DynamicProxy.IInterceptor
	{
		public IAdviceRegistry AdviceRegistry { get; set; }
		public IInjectorFactory InjectorFactory { get; set; }

		public Dispatcher(IAdviceRegistry adviceRegistry, IInjectorFactory injectorFactory)
		{
			AdviceRegistry = adviceRegistry;
			InjectorFactory = injectorFactory;
		}

		public Invocation CreateInvocation(MethodCall methodCall)
		{
			var interceptors = AdviceRegistry.GetInterceptors(methodCall);
			var injector = InjectorFactory.GetMethodInjector(methodCall.TargetMethod);

			return new Invocation(methodCall, interceptors, injector);
		}

		object LinFu.DynamicProxy.IInterceptor.Intercept(InvocationInfo info)
		{
			var methodCall = new MethodCall(info.Target, info.TargetMethod, info.CallingMethod, info.Arguments);

			Invocation invocation = CreateInvocation(methodCall);
			invocation.Proceed();

			return invocation.ReturnValue;
		}
	}
}