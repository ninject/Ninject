using System;
using System.Collections.Generic;
using Ninject.Injection.Injectors;

namespace Ninject.Interception
{
	public class Invocation
	{
		private readonly IEnumerator<IInterceptor> _enumerator;

		public MethodCall MethodCall { get; set; }
		public IEnumerable<IInterceptor> Interceptors { get; set; }
		public IMethodInjector MethodInjector { get; set; }
		public object ReturnValue { get; set; }

		public Invocation(MethodCall methodCall, IEnumerable<IInterceptor> interceptors, IMethodInjector methodInjector)
		{
			MethodCall = methodCall;
			Interceptors = interceptors;
			MethodInjector = methodInjector;

			_enumerator = interceptors.GetEnumerator();
		}

		public void Proceed()
		{
			if (_enumerator != null && _enumerator.MoveNext())
				_enumerator.Current.Intercept(this);
			else
				ReturnValue = MethodInjector.Invoke(MethodCall.Target, MethodCall.Arguments);
		}
	}
}