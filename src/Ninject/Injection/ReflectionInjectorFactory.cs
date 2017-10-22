// -------------------------------------------------------------------------------------------------
// <copyright file="ReflectionInjectorFactory.cs" company="Ninject Project Contributors">
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
    using System.Reflection;

    using Ninject.Components;

    /// <summary>
    /// Creates injectors from members via reflective invocation.
    /// </summary>
    public class ReflectionInjectorFactory : NinjectComponent, IInjectorFactory
    {
        /// <summary>
        /// Gets or creates an injector for the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created injector.</returns>
        public ConstructorInjector Create(ConstructorInfo constructor)
        {
            return args => constructor.Invoke(args);
        }

        /// <summary>
        /// Gets or creates an injector for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The created injector.</returns>
        public PropertyInjector Create(PropertyInfo property)
        {
            return (target, value) => property.SetValue(target, value, null);
        }

        /// <summary>
        /// Gets or creates an injector for the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The created injector.</returns>
        public MethodInjector Create(MethodInfo method)
        {
            return (target, args) => method.Invoke(target, args);
        }
    }
}