using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Interception.Advice;

namespace Ninject.Interception
{
	public class AdviceRegistry : IAdviceRegistry
	{
		private readonly List<IAdvice> _advice = new List<IAdvice>();

		public void Register(IAdvice advice)
		{
			_advice.Add(advice);
		}

		public bool HasAdvice(Type type)
		{
			return _advice.Any(advice => advice.Matches(type));
		}

		public IEnumerable<IInterceptor> GetInterceptors(MethodCall methodCall)
		{
			return _advice.Where(advice => advice.Matches(methodCall)).Select(advice => advice.GetInterceptor(methodCall));
		}
	}
}