// -------------------------------------------------------------------------------------------------
// <copyright file="Selector.cs" company="Ninject Project Contributors">
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

namespace Ninject.Selection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Selects members for injection.
    /// </summary>
    public class Selector : NinjectComponent, ISelector
    {
        private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// The injection heuristics.
        /// </summary>
        private readonly List<IInjectionHeuristic> injectionHeuristics;

        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// </summary>
        /// <param name="injectionHeuristics">The injection heuristics.</param>
        public Selector(IEnumerable<IInjectionHeuristic> injectionHeuristics)
        {
            Ensure.ArgumentNotNull(injectionHeuristics, "injectionHeuristics");

            this.injectionHeuristics = injectionHeuristics.ToList();
        }

        /// <summary>
        /// Gets the default binding flags.
        /// </summary>
        protected virtual BindingFlags Flags
        {
            get
            {
#if !NO_LCG
                return this.Settings.InjectNonPublic ? (DefaultFlags | BindingFlags.NonPublic) : DefaultFlags;
#else
                return DefaultFlags;
#endif
            }
        }

        /// <summary>
        /// Selects the constructor to call on the specified type, by using the constructor scorer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected constructor.</returns>
        public virtual ConstructorInfo[] SelectConstructorsForInjection(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");

            if (type.IsSubclassOf(typeof(MulticastDelegate)))
            {
                return Array.Empty<ConstructorInfo>();
            }

            return type.GetConstructors(this.Flags);
        }

        /// <summary>
        /// Selects properties that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected properties.</returns>
        public virtual IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");

            var bindingFlags = this.Flags;
            var declaredPropertiesToInject = type.GetProperties(bindingFlags)
                                                 .Select(p => p.GetPropertyFromDeclaredType(p, bindingFlags))
                                                 .Where(p => p != null && ShouldInject(this.injectionHeuristics, p));

            if (this.Settings.InjectParentPrivateProperties)
            {
                var properties = new List<PropertyInfo>(declaredPropertiesToInject);

                for (Type parentType = type.BaseType; parentType != null; parentType = parentType.BaseType)
                {
                    properties.AddRange(GetPrivateProperties(this.injectionHeuristics, parentType, bindingFlags));
                }

                return properties;
            }

            return declaredPropertiesToInject;
        }

        /// <summary>
        /// Selects methods that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected methods.</returns>
        public virtual IEnumerable<MethodInfo> SelectMethodsForInjection(Type type)
        {
            Ensure.ArgumentNotNull(type, "type");

            return type.GetMethods(this.Flags).Where(m => ShouldInject(this.injectionHeuristics, m));
        }

        private static bool ShouldInject(List<IInjectionHeuristic> injectionHeuristics, PropertyInfo property)
        {
            var shouldInject = false;

            foreach (var injectionHeuristic in injectionHeuristics)
            {
                if (injectionHeuristic.ShouldInject(property))
                {
                    shouldInject = true;
                    break;
                }
            }

            return shouldInject;
        }

        private static bool ShouldInject(List<IInjectionHeuristic> injectionHeuristics, MethodInfo method)
        {
            var shouldInject = false;

            foreach (var injectionHeuristic in injectionHeuristics)
            {
                if (injectionHeuristic.ShouldInject(method))
                {
                    shouldInject = true;
                    break;
                }
            }

            return shouldInject;
        }

        private static IEnumerable<PropertyInfo> GetPrivateProperties(List<IInjectionHeuristic> injectionHeuristics, Type type, BindingFlags bindingFlags)
        {
            return type.GetProperties(bindingFlags)
                       .Where(p => p.DeclaringType == type && p.IsPrivate() && ShouldInject(injectionHeuristics, p));
        }
    }
}