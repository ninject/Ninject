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
    using System.Linq;
    using System.Reflection;

    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Introspection;
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
        /// Initializes a new instance of the <see cref="PropertyInjectionStrategy"/> class.
        /// </summary>
        /// <param name="injectorFactory">The injector factory component.</param>
        public PropertyInjectionStrategy(IInjectorFactory injectorFactory)
        {
            this.InjectorFactory = injectorFactory;
        }

        /// <summary>
        /// Gets or sets the injector factory component.
        /// </summary>
        public IInjectorFactory InjectorFactory { get; set; }

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

            var propertyValues = context.Parameters.OfType<IPropertyValue>().ToList();

            foreach (var directive in context.Plan.GetAll<PropertyInjectionDirective>())
            {
                var value = this.GetValue(context, directive.Target, propertyValues);
                directive.Injector(reference.Instance, value);
            }

            this.AssignPropertyOverrides(context, reference, propertyValues);
        }

        /// <summary>
        /// Applies user supplied override values to instance properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        /// <param name="propertyValues">The parameter override value accessors.</param>
        private void AssignPropertyOverrides(IContext context, InstanceReference reference, IList<IPropertyValue> propertyValues)
        {
            var properties = reference.Instance.GetType().GetProperties(this.Flags);

            foreach (var propertyValue in propertyValues)
            {
                var propertyName = propertyValue.Name;
                var propertyInfo = properties.FirstOrDefault(property => string.Equals(property.Name, propertyName, StringComparison.Ordinal));

                if (propertyInfo == null)
                {
                    throw new ActivationException(ExceptionFormatter.CouldNotResolvePropertyForValueInjection(context.Request, propertyName));
                }

                var target = new PropertyInjectionDirective(propertyInfo, this.InjectorFactory.Create(propertyInfo));
                var value = this.GetValue(context, target.Target, propertyValues);
                target.Injector(reference.Instance, value);
            }
        }

        /// <summary>
        /// Gets the value to inject into the specified target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <param name="allPropertyValues">all property values of the current request.</param>
        /// <returns>The value to inject into the specified target.</returns>
        private object GetValue(IContext context, ITarget target, IEnumerable<IPropertyValue> allPropertyValues)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(target, "target");

            var parameter = allPropertyValues.SingleOrDefault(p => p.Name == target.Name);
            return parameter != null ? parameter.GetValue(context, target) : target.ResolveWithin(context);
        }
    }
}