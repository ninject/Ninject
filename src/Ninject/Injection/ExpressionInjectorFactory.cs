// -------------------------------------------------------------------------------------------------
// <copyright file="ExpressionInjectorFactory.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2019 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Injection
{
    using System.Linq.Expressions;
    using System.Reflection;

    using Ninject.Components;

    /// <summary>
    /// Creates injectors from members via expression invocation.
    /// </summary>
    public class ExpressionInjectorFactory : NinjectComponent, IInjectorFactory
    {
        /// <summary>
        /// Creates an injector for the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>
        /// The created injector.
        /// </returns>
        public ConstructorInjector Create(ConstructorInfo constructor)
        {
            var parameterArrayExpression = Expression.Parameter(typeof(object[]));
            var parameters = constructor.GetParameters();
            var typedParameterExpressions = CreateTypedParameterExpressions(parameters, parameterArrayExpression);

            var lambda = Expression.Lambda<ConstructorInjector>(
                Expression.New(constructor, typedParameterExpressions),
                parameterArrayExpression);

            return lambda.Compile();
        }

        /// <summary>
        /// Creates an injector for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// The created injector.
        /// </returns>
        public PropertyInjector Create(PropertyInfo property)
        {
            var targetExpression = Expression.Parameter(typeof(object));
            var typedTargetExpression = property.ReflectedType.IsValueType ?
                Expression.Unbox(targetExpression, property.ReflectedType) :
                Expression.TypeAs(targetExpression, property.ReflectedType);

            var parameterExpression = Expression.Parameter(typeof(object));
            var typedParameterExpression = property.PropertyType.IsValueType ?
                Expression.Unbox(parameterExpression, property.PropertyType) :
                Expression.TypeAs(parameterExpression, property.PropertyType);

            var lambda = Expression.Lambda<PropertyInjector>(
                Expression.Assign(Expression.Property(typedTargetExpression, property), typedParameterExpression),
                targetExpression,
                parameterExpression);

            return lambda.Compile();
        }

        /// <summary>
        /// Creates an injector for the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        /// The created injector.
        /// </returns>
        public MethodInjector Create(MethodInfo method)
        {
            var targetExpression = Expression.Parameter(typeof(object));
            var typedTargetExpression = method.ReflectedType.IsValueType ?
                Expression.Unbox(targetExpression, method.ReflectedType) :
                Expression.TypeAs(targetExpression, method.ReflectedType);

            var parameterArrayExpression = Expression.Parameter(typeof(object[]));
            var parameters = method.GetParameters();
            var typedParameterExpressions = CreateTypedParameterExpressions(parameters, parameterArrayExpression);

            var lambda = Expression.Lambda<MethodInjector>(
                Expression.Call(typedTargetExpression, method, typedParameterExpressions),
                targetExpression,
                parameterArrayExpression);

            return lambda.Compile();
        }

        private static UnaryExpression[] CreateTypedParameterExpressions(ParameterInfo[] parameters, ParameterExpression parameterArrayExpression)
        {
            var typedParameterExpressions = new UnaryExpression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;

                var parameterExpression = Expression.ArrayIndex(parameterArrayExpression, Expression.Constant(i));
                typedParameterExpressions[i] = parameterType.IsValueType ?
                                               Expression.Unbox(parameterExpression, parameterType) :
                                               Expression.TypeAs(parameterExpression, parameterType);
            }

            return typedParameterExpressions;
        }
    }
}