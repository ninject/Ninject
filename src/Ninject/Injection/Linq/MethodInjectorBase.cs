using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ninject.Injection.Linq
{
	/// <summary>
	/// An expression-based injector that can inject values into a constructor or method.
	/// </summary>
	/// <typeparam name="TDelegate">The type of delegate resulting from the expression tree.</typeparam>
	public abstract class MethodInjectorBase<TDelegate> : ExpressionBasedInjector<MethodInfo, TDelegate>, IMethodInjector
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjectorBase&lt;TDelegate&gt;"/> class.
		/// </summary>
		/// <param name="method">The method that will be injected.</param>
		protected MethodInjectorBase(MethodInfo method) : base(method) { }

		/// <summary>
		/// Calls the associated method, injecting the specified values.
		/// </summary>
		/// <param name="target">The target object on which to call the method.</param>
		/// <param name="values">The values to inject.</param>
		/// <returns>The return value of the method, or <see langword="null"/> if the method returns <see type="void"/>.</returns>
		public abstract object Invoke(object target, params object[] values);

		/// <summary>
		/// Builds the expression tree that can be compiled into a delegate, which in turn
		/// can be used to inject values into the member.
		/// </summary>
		/// <param name="member">The member that will be injected.</param>
		/// <returns>The constructed expression tree.</returns>
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