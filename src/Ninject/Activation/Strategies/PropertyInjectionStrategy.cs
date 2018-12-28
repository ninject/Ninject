// -------------------------------------------------------------------------------------------------
// <copyright file="PropertyInjectionStrategy.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Injection;
    using Ninject.Parameters;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Injects properties on an instance during activation.
    /// </summary>
    public class PropertyInjectionStrategy : ActivationStrategy
    {
        private const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// The injector factory component.
        /// </summary>
        private readonly IInjectorFactory injectorFactory;

        /// <summary>
        /// The <see cref="IExceptionFormatter"/> component.
        /// </summary>
        private readonly IExceptionFormatter exceptionFormatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInjectionStrategy"/> class.
        /// </summary>
        /// <param name="injectorFactory">The injector factory component.</param>
        /// <param name="exceptionFormatter">The <see cref="IExceptionFormatter"/> component.</param>
        public PropertyInjectionStrategy(IInjectorFactory injectorFactory, IExceptionFormatter exceptionFormatter)
        {
            this.injectorFactory = injectorFactory;
            this.exceptionFormatter = exceptionFormatter;
        }

        private BindingFlags Flags
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
        /// Injects values into the properties as described by <see cref="PropertyInjectionDirective"/>s
        /// contained in the plan.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public override void Activate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(reference, "reference");

            var propertyValues = GetPropertyValues(context.Parameters);

            foreach (var directive in context.Plan.GetAll<PropertyInjectionDirective>())
            {
                var value = this.GetValue(context, directive.Target, propertyValues);
                directive.Injector(reference.Instance, value);
            }

            if (propertyValues.Count > 0)
            {
                this.AssignPropertyOverrides(context, reference, propertyValues);
            }
        }

        /// <summary>
        /// Locates a <see cref="PropertyInfo"/> by name using the specified <see cref="StringComparison"/>.
        /// </summary>
        /// <param name="properties">The list of properties to search.</param>
        /// <param name="name">The name to find.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> to use when comparing the name.</param>
        /// <returns>
        /// The <see cref="PropertyInfo"/> with a matching <see cref="IParameter.Name"/>, if found; otherwise,
        /// <see langword="null"/>.
        /// </returns>
        private static PropertyInfo FindPropertyByName(PropertyInfo[] properties, string name, StringComparison stringComparison)
        {
            PropertyInfo found = null;

            foreach (var property in properties)
            {
                if (string.Equals(property.Name, name, stringComparison))
                {
                    found = property;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Returns the <see cref="IPropertyValue"/> instances in the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters to search.</param>
        /// <returns>
        /// A list of the <see cref="IPropertyValue"/> instances in <paramref name="parameters"/>.
        /// </returns>
        private static List<IPropertyValue> GetPropertyValues(IEnumerable<IParameter> parameters)
        {
            var propertyValues = new List<IPropertyValue>();

            foreach (var parameter in parameters)
            {
                if (parameter is IPropertyValue propertyValue)
                {
                    propertyValues.Add(propertyValue);
                }
            }

            return propertyValues;
        }

        /// <summary>
        /// Applies user supplied override values to instance properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        /// <param name="propertyValues">The parameter override value accessors.</param>
        private void AssignPropertyOverrides(IContext context, InstanceReference reference, List<IPropertyValue> propertyValues)
        {
            var properties = reference.Instance.GetType().GetProperties(this.Flags);

            foreach (var propertyValue in propertyValues)
            {
                var propertyInfo = FindPropertyByName(properties, propertyValue.Name, StringComparison.Ordinal);

                if (propertyInfo == null)
                {
                    throw new ActivationException(this.exceptionFormatter.CouldNotResolvePropertyForValueInjection(context.Request, propertyValue.Name));
                }

                var target = new PropertyInjectionDirective(propertyInfo, this.injectorFactory.Create(propertyInfo));
                var value = propertyValue.GetValue(context, target.Target);
                target.Injector(reference.Instance, value);
            }
        }

        /// <summary>
        /// Gets the value to inject into the specified target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <param name="allPropertyValues">The property values of the current request.</param>
        /// <returns>
        /// The value to inject into the specified <see cref="ITarget"/>.
        /// </returns>
        private object GetValue(IContext context, ITarget target, List<IPropertyValue> allPropertyValues)
        {
            var parameter = this.FindPropertyValueByName(context, allPropertyValues, target.Name, StringComparison.Ordinal);
            if (parameter != null)
            {
                // Remove parameter from list of property values to ensure we do not attempt to process
                // it again when we assign the overrides.
                allPropertyValues.Remove(parameter);
                return parameter.GetValue(context, target);
            }

            return target.ResolveWithin(context);
        }

        /// <summary>
        /// Locates a <see cref="IPropertyValue"/> by name using the specified <see cref="StringComparison"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValues">The list of property values to search.</param>
        /// <param name="name">The name to find.</param>
        /// <param name="stringComparison">The <see cref="StringComparison"/> to use when comparing the name.</param>
        /// <returns>
        /// The <see cref="IPropertyValue"/> with a matching <see cref="IParameter.Name"/>, if found; otherwise,
        /// <see langword="null"/>.
        /// </returns>
        private IPropertyValue FindPropertyValueByName(IContext context, List<IPropertyValue> propertyValues, string name, StringComparison stringComparison)
        {
            IPropertyValue found = null;

            foreach (var propertyValue in propertyValues)
            {
                if (string.Equals(propertyValue.Name, name, stringComparison))
                {
                    found = propertyValue;
                    break;
                }
            }

            return found;
        }
    }
}