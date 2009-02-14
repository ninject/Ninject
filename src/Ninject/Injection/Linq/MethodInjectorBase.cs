#region License
// Author: Nate Kohari <nkohari@gmail.com>
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
#endregion

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

			ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");
			var arguments = MethodInjectionExpressionHelper.CreateParameterExpressions(member, argumentsParameter);

			MethodCallExpression call = Expression.Call(instance, member, arguments);

			return Expression.Lambda<TDelegate>(call, instanceParameter, argumentsParameter);
		}
	}
}