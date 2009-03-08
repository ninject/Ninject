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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;
#endregion

namespace Ninject.Injection
{
	/// <summary>
	/// Creates injectors for members via expression trees.
	/// </summary>
	public class ExpressionInjectorFactory : NinjectComponent, IInjectorFactory
	{
		private readonly Dictionary<MemberKey, ConstructorInjector> _constructorInjectors = new Dictionary<MemberKey, ConstructorInjector>();
		private readonly Dictionary<MemberKey, PropertyInjector> _propertyInjectors = new Dictionary<MemberKey, PropertyInjector>();
		private readonly Dictionary<MemberKey, MethodInjector> _methodInjectors = new Dictionary<MemberKey, MethodInjector>();

		/// <summary>
		/// Gets or creates an injector for the specified constructor.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <returns>The created injector.</returns>
		public ConstructorInjector GetInjector(ConstructorInfo constructor)
		{
			Ensure.ArgumentNotNull(constructor, "constructor");

			var key = new MemberKey(constructor);

			if (_constructorInjectors.ContainsKey(key))
				return _constructorInjectors[key];

			var injector = Create(constructor);
			_constructorInjectors[key] = injector;

			return injector;
		}

		/// <summary>
		/// Gets or creates an injector for the specified property.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <returns>The created injector.</returns>
		public PropertyInjector GetInjector(PropertyInfo property)
		{
			Ensure.ArgumentNotNull(property, "property");

			var key = new MemberKey(property);

			if (_propertyInjectors.ContainsKey(key))
				return _propertyInjectors[key];

			var injector = Create(property);
			_propertyInjectors[key] = injector;

			return injector;
		}

		/// <summary>
		/// Gets or creates an injector for the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The created injector.</returns>
		public MethodInjector GetInjector(MethodInfo method)
		{
			Ensure.ArgumentNotNull(method, "method");

			var key = new MemberKey(method);

			if (_methodInjectors.ContainsKey(key))
				return _methodInjectors[key];

			var injector = Create(method);
			_methodInjectors[key] = injector;

			return injector;
		}

		private static ConstructorInjector Create(ConstructorInfo constructor)
		{
			ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

			NewExpression call = Expression.New(constructor,
				CreateParameterExpressions(constructor, argumentsParameter));

			var expression = Expression.Lambda<ConstructorInjector>(call, argumentsParameter);

			return expression.Compile();
		}

		private static MethodInjector Create(MethodInfo method)
		{
			ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
			ParameterExpression argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

			MethodCallExpression call = Expression.Call(
				Expression.Convert(instanceParameter, method.DeclaringType),
				method,
				CreateParameterExpressions(method, argumentsParameter));

			var expression = Expression.Lambda<MethodInjector>(call, instanceParameter, argumentsParameter);

			return expression.Compile();
		}

		private static PropertyInjector Create(PropertyInfo member)
		{
			ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
			ParameterExpression argumentParameter = Expression.Parameter(typeof(object), "value");

			Expression call = Expression.Call(
				Expression.Convert(instanceParameter, member.DeclaringType),
				member.GetSetMethod(),
				Expression.Convert(argumentParameter, member.PropertyType));

			var expression = Expression.Lambda<PropertyInjector>(call, instanceParameter, argumentParameter);

			return expression.Compile();
		}

		private static Expression[] CreateParameterExpressions(MethodBase method, Expression argumentParameter)
		{
			return method.GetParameters().Select((parameter, index) =>
				Expression.Convert(
					Expression.ArrayIndex(argumentParameter, Expression.Constant(index)),
					parameter.ParameterType)).ToArray();
		}
	}
}