// -------------------------------------------------------------------------------------------------
// <copyright file="StandardInjectionHeuristic.cs" company="Ninject Project Contributors">
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

namespace Ninject.Selection.Heuristics
{
    using System.Reflection;

    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;

    /// <summary>
    /// Determines whether members should be injected during activation by checking
    /// if they are decorated with an injection marker attribute.
    /// </summary>
    public class StandardInjectionHeuristic : NinjectComponent, IInjectionHeuristic
    {
        /// <summary>
        /// The ninject settings.
        /// </summary>
        private readonly INinjectSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardInjectionHeuristic"/> class.
        /// </summary>
        /// <param name="settings">The ninject settings.</param>
        public StandardInjectionHeuristic(INinjectSettings settings)
        {
            Ensure.ArgumentNotNull(settings, nameof(settings));

            this.settings = settings;
        }

        /// <summary>
        /// Returns a value indicating whether the specified property should be injected.
        /// </summary>
        /// <param name="property">The property in question.</param>
        /// <returns><c>true</c> if the property should be injected; otherwise <c>false</c>.</returns>
        public virtual bool ShouldInject(PropertyInfo property)
        {
            Ensure.ArgumentNotNull(property, nameof(property));

            var setMethod = property.GetSetMethod(this.settings.InjectNonPublic);

            return setMethod != null && property.HasAttribute(this.settings.InjectAttribute);
        }

        /// <summary>
        /// Returns a value indicating whether the specified method should be injected.
        /// </summary>
        /// <param name="method">The method in question.</param>
        /// <returns><c>true</c> if the method should be injected; otherwise <c>false</c>.</returns>
        public virtual bool ShouldInject(MethodInfo method)
        {
            Ensure.ArgumentNotNull(method, nameof(method));

            return method.HasAttribute(this.settings.InjectAttribute);
        }
    }
}