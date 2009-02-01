using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ninject.Injection.Injectors.Linq
{
	public class ConstructorInjector : ExpressionInjectorBase<ConstructorInfo, Func<object[], object>>, IConstructorInjector
	{
		public ConstructorInjector(ConstructorInfo constructor) : base(constructor) { }

		public object Invoke(params object[] values)
		{
			return Callback.Invoke(values);
		}

		protected override Expression<Func<object[], object>> BuildExpression(ConstructorInfo member)
		{
			ParameterExpression argumentParameter = Expression.Parameter(typeof(object[]), "arguments");

			ParameterInfo[] parameters = member.GetParameters();
			Expression[] arguments = new Expression[parameters.Length];

			for (int idx = 0; idx < parameters.Length; idx++)
			{
				arguments[idx] = Expression.Convert(
					Expression.ArrayIndex(argumentParameter, Expression.Constant(idx)),
					parameters[idx].ParameterType);
			}

			NewExpression newCall = Expression.New(member, arguments);

			return Expression.Lambda<Func<object[], object>>(newCall, argumentParameter);
		}
	}
}