using System;
using System.Reflection;

namespace Ninject.Interception
{
	public class MethodCall
	{
		public object Target { get; set; }
		public MethodInfo TargetMethod { get; set; }
		public MethodInfo CallingMethod { get; set; }
		public object[] Arguments { get; set; }
		public Type[] GenericArguments { get; private set; }

		public bool HasGenericArguments
		{
			get { return GenericArguments != null; }
		}

		public MethodCall(object target, MethodInfo targetMethod, MethodInfo callingMethod, object[] arguments)
		{
			Target = target;
			TargetMethod = targetMethod;
			CallingMethod = callingMethod;
			Arguments = arguments;

			if (targetMethod.IsGenericMethod)
				GenericArguments = targetMethod.GetGenericArguments();
		}
	}
}