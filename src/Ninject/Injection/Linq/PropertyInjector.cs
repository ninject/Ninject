#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Linq.Expressions;
using System.Reflection;
#endregion

namespace Ninject.Injection.Linq
{
	/// <summary>
	/// An injector that injects values into a property.
	/// </summary>
	public class PropertyInjector : ExpressionBasedInjector<PropertyInfo, Action<object, object>>, IPropertyInjector
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyInjector"/> class.
		/// </summary>
		/// <param name="property">The property that will be injected.</param>
		public PropertyInjector(PropertyInfo property) : base(property) { }

		/// <summary>
		/// Injects the specified value into the property.
		/// </summary>
		/// <param name="target">The target object to inject.</param>
		/// <param name="value">The value to inject.</param>
		public void Invoke(object target, object value)
		{
			Callback.Invoke(target, value);
		}

		/// <summary>
		/// Builds the expression tree that can be compiled into a delegate, which in turn
		/// can be used to inject values into the member.
		/// </summary>
		/// <param name="member">The member that will be injected.</param>
		/// <returns>The constructed expression tree.</returns>
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