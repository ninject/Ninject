// -------------------------------------------------------------------------------------------------
// <copyright file="StandardProvider.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Providers
{
    using System;
    using System.Linq;

    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// The standard provider for types, which activates instances via a <see cref="IPipeline"/>.
    /// </summary>
    public class StandardProvider : IProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardProvider"/> class.
        /// </summary>
        /// <param name="type">The type (or prototype) of instances the provider creates.</param>
        /// <param name="planner">The planner component.</param>
        /// <param name="constructorScorer">The constructor scorer component.</param>
        public StandardProvider(Type type, IPlanner planner, IConstructorScorer constructorScorer)
        {
            Ensure.ArgumentNotNull(type, "type");
            Ensure.ArgumentNotNull(planner, "planner");
            Ensure.ArgumentNotNull(constructorScorer, "constructorScorer");

            this.Type = type;
            this.Planner = planner;
            this.ConstructorScorer = constructorScorer;
        }

        /// <summary>
        /// Gets the type (or prototype) of instances the provider creates.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the planner component.
        /// </summary>
        public IPlanner Planner { get; private set; }

        /// <summary>
        /// Gets the constructor scorer component.
        /// </summary>
        public IConstructorScorer ConstructorScorer { get; private set; }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        public virtual object Create(IContext context)
        {
            Ensure.ArgumentNotNull(context, "context");

            if (context.Plan == null)
            {
                context.Plan = this.Planner.GetPlan(this.GetImplementationType(context.Request.Service));
            }

            var directive = this.DetermineConstructorInjectionDirective(context);
            var arguments = GetValues(context, directive.Targets);
            return directive.Injector(arguments);
        }

        /// <summary>
        /// Gets the value to inject into the specified target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>The value to inject into the specified target.</returns>
        public object GetValue(IContext context, ITarget target)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(target, "target");

            return GetValueCore(context, target);
        }

        /// <summary>
        /// Gets the implementation type that the provider will activate an instance of
        /// for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>The implementation type that will be activated.</returns>
        public Type GetImplementationType(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");

            return this.Type.ContainsGenericParameters ? this.Type.MakeGenericType(service.GetGenericArguments()) : this.Type;
        }

        private static object GetValueCore(IContext context, ITarget target)
        {
            IConstructorArgument constructorArgument = null;

            foreach (var parameter in context.Parameters)
            {
                if (parameter is IConstructorArgument ctorArg && ctorArg.AppliesToTarget(context, target))
                {
                    if (constructorArgument != null)
                    {
                        throw new InvalidOperationException("Sequence contains more than one matching element");
                    }

                    constructorArgument = ctorArg;
                }
            }

            if (constructorArgument != null)
            {
                return constructorArgument.GetValue(context, target);
            }

            return target.ResolveWithin(context);
        }

        private static object[] GetValues(IContext context, ITarget[] targets)
        {
            if (targets.Length == 0)
            {
                return Array.Empty<object>();
            }

            object[] values = new object[targets.Length];

            for (var i = 0; i < targets.Length; i++)
            {
                values[i] = GetValueCore(context, targets[i]);
            }

            return values;
        }

        private ConstructorInjectionDirective DetermineConstructorInjectionDirective(IContext context)
        {
            var directives = context.Plan.GetAll<ConstructorInjectionDirective>().ToArray();

            if (directives.Length == 0)
            {
                throw new ActivationException(ExceptionFormatter.NoConstructorsAvailable(context));
            }

            if (directives.Length == 1)
            {
                return directives[0];
            }

            var bestDirectives =
                directives
                    .GroupBy(directive => this.ConstructorScorer.Score(context, directive))
                    .OrderByDescending(g => g.Key)
                    .First()
                    .ToArray();

            if (bestDirectives.Length > 1)
            {
                throw new ActivationException(ExceptionFormatter.ConstructorsAmbiguous(context, bestDirectives));
            }

            return bestDirectives[0];
        }
    }
}