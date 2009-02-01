using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ninject.Injection.Injectors.Linq
{
	public class PropertyInjector : ExpressionInjectorBase<PropertyInfo, Action<object, object>>, IPropertyInjector
	{
		public PropertyInjector(PropertyInfo property) : base(property) { }

		public void Invoke(object target, object value)
		{
			Callback.Invoke(target, value);
		}

		protected override Expression<Action<object, object>> BuildExpression(PropertyInfo member)
		{
			ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
			Expression instance = Expression.Convert(instanceParameter, member.DeclaringType);

			ParameterExpression argumentParameter = Expression.Parameter(typeof(object), "value");
			Expression argument = Expression.Convert(argumentParameter, member.PropertyType);

			Expression call = Expression.Call(instance, member.GetSetMethod(), argument);

			return Expression.Lambda<Action<object, object>>(call, instanceParameter, argumentParameter);
		}
	}
}