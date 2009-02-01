using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ninject.Injection.Injectors.Linq
{
	public abstract class MethodInjectorBase<TDelegate> : ExpressionInjectorBase<MethodInfo, TDelegate>, IMethodInjector
	{
		protected MethodInjectorBase(MethodInfo method) : base(method) { }

		public abstract object Invoke(object target, params object[] values);

		protected override Expression<TDelegate> BuildExpression(MethodInfo member)
		{
			ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
			Expression instance = Expression.Convert(instanceParameter, member.DeclaringType);

			ParameterExpression argumentParameter = Expression.Parameter(typeof(object[]), "arguments");

			ParameterInfo[] parameters = member.GetParameters();
			Expression[] arguments = new Expression[parameters.Length];

			for (int idx = 0; idx < parameters.Length; idx++)
			{
				arguments[idx] = Expression.Convert(
					Expression.ArrayIndex(argumentParameter, Expression.Constant(idx)),
					parameters[idx].ParameterType);
			}

			MethodCallExpression call = Expression.Call(instance, member, arguments);

			return Expression.Lambda<TDelegate>(call, instanceParameter, argumentParameter);
		}
	}
}