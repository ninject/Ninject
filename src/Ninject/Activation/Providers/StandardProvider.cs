//-------------------------------------------------------------------------------------------------
// <copyright file="StandardProvider.cs" company="Ninject Project Contributors">
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
namespace Ninject.Activation.Providers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using Ninject.Infrastructure.Introspection;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Directives;
    using Ninject.Planning.Targets;
    using Ninject.Selection;
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
            Contract.Requires(type != null);
            Contract.Requires(planner != null);
            Contract.Requires(constructorScorer != null);

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
        /// Gets the selector component.
        /// </summary>
        public IConstructorScorer ConstructorScorer { get; private set; }

        /// <summary>
        /// Gets a callback that creates an instance of the <see cref="StandardProvider"/>
        /// for the specified type.
        /// </summary>
        /// <param name="prototype">The prototype the provider instance will create.</param>
        /// <returns>The created callback.</returns>
        public static Func<IContext, IProvider> GetCreationCallback(Type prototype)
        {
            Contract.Requires(prototype != null);
            return ctx => new StandardProvider(prototype, ctx.Kernel.Components.Get<IPlanner>(), ctx.Kernel.Components.Get<ISelector>().ConstructorScorer);
        }

        /// <summary>
        /// Gets a callback that creates an instance of the <see cref="StandardProvider"/>
        /// for the specified type and constructor.
        /// </summary>
        /// <param name="prototype">The prototype the provider instance will create.</param>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The created callback.</returns>
        public static Func<IContext, IProvider> GetCreationCallback(Type prototype, ConstructorInfo constructor)
        {
            Contract.Requires(prototype != null);
            return ctx => new StandardProvider(prototype, ctx.Kernel.Components.Get<IPlanner>(), new SpecificConstructorSelector(constructor));
        }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        public virtual object Create(IContext context)
        {
            Contract.Requires(context != null);

            if (context.Plan == null)
            {
                context.Plan = this.Planner.GetPlan(this.GetImplementationType(context.Request.Service));
            }

            ConstructorInjectionDirective directive = null;
            var directives = context.Plan.ConstructorInjectionDirectives;

            if (directives.Count == 1)
            {
                directive = directives[0];
            }
            else
            {
                var bestDirectives = directives
                      .GroupBy(option => this.ConstructorScorer.Score(context, option))
                      .OrderByDescending(g => g.Key)
                      .FirstOrDefault();
                if (bestDirectives == null)
                {
                    throw new ActivationException(ExceptionFormatter.NoConstructorsAvailable(context));
                }

                if (bestDirectives.Skip(1).Any())
                {
                    throw new ActivationException(ExceptionFormatter.ConstructorsAmbiguous(context, bestDirectives));
                }

                directive = bestDirectives.First();
            }

            var arguments = directive.Targets.Select(target => this.GetValue(context, target)).ToArray();
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
            Contract.Requires(context != null);
            Contract.Requires(target != null);

            var parameter = context
                .Parameters.OfType<IConstructorArgument>()
               .SingleOrDefault(p => p.AppliesToTarget(context, target));
            return parameter != null ? parameter.GetValue(context, target) : target.ResolveWithin(context);
        }

        /// <summary>
        /// Gets the implementation type that the provider will activate an instance of
        /// for the specified service.
        /// </summary>
        /// <param name="service">The service in question.</param>
        /// <returns>The implementation type that will be activated.</returns>
        public Type GetImplementationType(Type service)
        {
            Contract.Requires(service != null);
            return this.Type.GetTypeInfo().ContainsGenericParameters ? this.Type.MakeGenericType(service.GetTypeInfo().GetGenericArguments()) : this.Type;
        }
    }
}