//-------------------------------------------------------------------------------------------------
// <copyright file="Selector.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2009, Enkari, Ltd.
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
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
//-------------------------------------------------------------------------------------------------
namespace Ninject.Selection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using Ninject.Components;
    using Ninject.Infrastructure.Language;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Selects members for injection.
    /// </summary>
    public class Selector : NinjectComponent, ISelector
    {
        private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// </summary>
        /// <param name="constructorScorer">The constructor scorer.</param>
        /// <param name="injectionHeuristics">The injection heuristics.</param>
        public Selector(IConstructorScorer constructorScorer, IEnumerable<IInjectionHeuristic> injectionHeuristics)
        {
            Contract.Requires(constructorScorer != null);
            Contract.Requires(injectionHeuristics != null);

            this.ConstructorScorer = constructorScorer;
            this.InjectionHeuristics = injectionHeuristics.ToList();
        }

        /// <summary>
        /// Gets or sets the constructor scorer.
        /// </summary>
        public IConstructorScorer ConstructorScorer { get; set; }

        /// <summary>
        /// Gets the property injection heuristics.
        /// </summary>
        public ICollection<IInjectionHeuristic> InjectionHeuristics { get; private set; }

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
        /// <returns>The selected constructor, or <see langword="null"/> if none were available.</returns>
        public virtual IEnumerable<ConstructorInfo> SelectConstructorsForInjection(Type type)
        {
            Contract.Requires(type != null);

            if (type.GetTypeInfo().IsSubclassOf(typeof(MulticastDelegate)))
            {
                return null;
            }

            var constructors = type.GetTypeInfo().GetConstructors(this.Flags);
            return constructors.Length == 0 ? null : constructors;
        }

        /// <summary>
        /// Selects properties that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected properties.</returns>
        public virtual IEnumerable<PropertyInfo> SelectPropertiesForInjection(Type type)
        {
            Contract.Requires(type != null);
            var properties = new List<PropertyInfo>();
            properties.AddRange(
                type.GetTypeInfo().GetProperties(this.Flags)
                       .Select(p => p.GetPropertyFromDeclaredType(p))
                       .Where(p => this.InjectionHeuristics.Any(h => h.ShouldInject(p))));

            if (this.Settings.InjectParentPrivateProperties)
            {
                for (Type parentType = type.GetTypeInfo().BaseType; parentType != null; parentType = parentType.GetTypeInfo().BaseType)
                {
                    properties.AddRange(this.GetPrivateProperties(type.GetTypeInfo().BaseType));
                }
            }

            return properties;
        }

        /// <summary>
        /// Selects methods that should be injected.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A series of the selected methods.</returns>
        public virtual IEnumerable<MethodInfo> SelectMethodsForInjection(Type type)
        {
            Contract.Requires(type != null);
            return type.GetTypeInfo().GetMethods(this.Flags).Where(m => this.InjectionHeuristics.Any(h => h.ShouldInject(m)));
        }

        private IEnumerable<PropertyInfo> GetPrivateProperties(Type type)
        {
            return type.GetTypeInfo().GetProperties(this.Flags).Where(p => p.DeclaringType == type && p.IsPrivate())
                .Where(p => this.InjectionHeuristics.Any(h => h.ShouldInject(p)));
        }
    }
}