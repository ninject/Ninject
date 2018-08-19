// -------------------------------------------------------------------------------------------------
// <copyright file="ExpressionInjectorFactory.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010 Enkari, Ltd. All rights reserved.
//   Copyright (c) 2010-2017 Ninject Project Contributors. All rights reserved.
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
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Ninject.Components;

    /// <summary>
    /// Creates injectors from members via expression invocation.
    /// </summary>
    public class ExpressionInjectorFactory : NinjectComponent, IInjectorFactory
    {
        private readonly ConcurrentDictionary<ConstructorInfo, Delegate> constructorCache = new ConcurrentDictionary<ConstructorInfo, Delegate>();
        private readonly ConcurrentDictionary<PropertyInfo, Delegate> propertyCache = new ConcurrentDictionary<PropertyInfo, Delegate>();
        private readonly ConcurrentDictionary<MethodInfo, Delegate> methodCache = new ConcurrentDictionary<MethodInfo, Delegate>();

        /// <summary>
        /// Gets or creates an injector for the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created injector.</returns>
        public ConstructorInjector Create(ConstructorInfo constructor)
        {
            var delegation = this.constructorCache.GetOrAdd(
                constructor,
                c =>
                {
                    var parameterExpressions = c.GetParameters().Select(p => Expression.Parameter(p.ParameterType)).ToArray();
                    var lambda = Expression.Lambda(Expression.New(c, parameterExpressions), parameterExpressions);
                    return lambda.Compile();
                });

            return args => delegation.DynamicInvoke(args);
        }

        /// <summary>
        /// Gets or creates an injector for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The created injector.</returns>
        public PropertyInjector Create(PropertyInfo property)
        {
            var delegation = this.propertyCache.GetOrAdd(
                property,
                p =>
                {
                    var targetExpression = Expression.Parameter(p.ReflectedType);
                    var valueExpression = Expression.Parameter(p.PropertyType);
                    var lambda = Expression.Lambda(Expression.Assign(Expression.Property(targetExpression, p), valueExpression), targetExpression, valueExpression);
                    return lambda.Compile();
                });

            return (target, value) => delegation.DynamicInvoke(target, value);
        }

        /// <summary>
        /// Gets or creates an injector for the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The created injector.</returns>
        public MethodInjector Create(MethodInfo method)
        {
            var delegation = this.methodCache.GetOrAdd(
                method,
                m =>
                {
                    var parameterExpressions = m.GetParameters().Select(p => Expression.Parameter(p.ParameterType)).ToArray();
                    var targetExpression = Expression.Parameter(m.ReflectedType);
                    var lambda = Expression.Lambda(Expression.Call(targetExpression, m, parameterExpressions), new ParameterExpression[] { targetExpression }.Concat(parameterExpressions));
                    return lambda.Compile();
                });

            return (target, args) => delegation.DynamicInvoke(new object[] { target }.Concat(args).ToArray());
        }
    }
}