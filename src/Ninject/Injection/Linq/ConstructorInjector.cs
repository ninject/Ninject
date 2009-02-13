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
using System.Linq.Expressions;
using System.Reflection;
#endregion

namespace Ninject.Injection.Linq
{
	/// <summary>
	/// An expression-based injector that injects values into a constructor.
	/// </summary>
	public class ConstructorInjector : ExpressionBasedInjector<ConstructorInfo, Func<object[], object>>, IConstructorInjector
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConstructorInjector"/> class.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		public ConstructorInjector(ConstructorInfo constructor) : base(constructor) { }

		/// <summary>
		/// Calls the associated constructor, injecting the specified values.
		/// </summary>
		/// <param name="values">The values to inject.</param>
		/// <returns>The object created by the constructor.</returns>
		public object Invoke(params object[] values)
		{
			return Callback.Invoke(values);
		}

		/// <summary>
		/// Builds the expression tree that can be compiled into a delegate, which in turn
		/// can be used to inject values into the member.
		/// </summary>
		/// <param name="member">The member that will be injected.</param>
		/// <returns>The constructed expression tree.</returns>
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