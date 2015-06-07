//-------------------------------------------------------------------------------
// <copyright file="StandardProvider.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2014 Ninject Project Contributors
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
//-------------------------------------------------------------------------------

namespace Ninject.Activation.Providers
{
using System;
using System.Linq;
    using System.Reflection;
using Ninject.Infrastructure.Introspection;
    using Ninject.Infrastructure.Language;
using Ninject.Parameters;
    using Ninject.Planning.Bindings;
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
        /// <param name="constructorScorer">The constructor scorer component.</param>
        public StandardProvider(Type type, IConstructorScorer constructorScorer)
        {
            this.Type = type;
            this.ConstructorScorer = constructorScorer;
        }

        /// <summary>
        /// Gets the type (or prototype) of instances the provider creates.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the selector component.
        /// </summary>
        public IConstructorScorer ConstructorScorer { get; private set; }

        /// <summary>
        /// Creates an instance within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created instance.</returns>
        public virtual object Create(IContext context)
        {
            context.BuildPlan(this.GetImplementationType(context.Request.Service));

            var directive = this.DetermineConstructorInjectionDirective(context);

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
            return Type.GetTypeInfo().ContainsGenericParameters ? 
                Type.MakeGenericType(service.GetTypeInfo().GenericTypeArguments) : 
                Type;
        }

        /// <summary>
        /// Gets a callback that creates an instance of the <see cref="StandardProvider"/>
        /// for the specified type.
        /// </summary>
        /// <param name="prototype">
        /// The prototype the provider instance will create.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// The created callback.
        /// </returns>
        public static Func<IContext, IProvider> GetCreationCallback(Type prototype, ISelector selector)
        {
            var provider = new StandardProvider(prototype, selector.ConstructorScorer);
            return ctx => provider;
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
            var provider = new StandardProvider(prototype, new SpecificConstructorSelector(constructor));
            return ctx => provider;
        }

        /// <summary>
        /// Assigns the provider callback to the building configuration.
        /// </summary>
        /// <param name="bindingConfiguration">
        /// The building configuration.
        /// </param>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        public static void AssignProviderCallback(IBindingConfiguration bindingConfiguration, Type prototype)
        {
            var provider = new StandardProvider(prototype, null);
            bindingConfiguration.ProviderCallback = ctx => provider;
            bindingConfiguration.InitializeProviderCallback =
                selector => provider.ConstructorScorer = selector.ConstructorScorer;
        }

        private ConstructorInjectionDirective DetermineConstructorInjectionDirective(IContext context)
        {
            var directives = context.Plan.ConstructorInjectionDirectives;
            if (directives.Count == 1)
            {
                return directives[0];
            }
            IGrouping<int, ConstructorInjectionDirective> bestDirectives =
                directives
                    .GroupBy(option => this.ConstructorScorer.Score(context, option))
                    .OrderByDescending(g => g.Key)
                    .FirstOrDefault();
            if (bestDirectives == null)
            {
                throw new ActivationException(ExceptionFormatter.NoConstructorsAvailable(context));
            }

            return bestDirectives.SingleOrThrowException(
                () => new ActivationException(ExceptionFormatter.ConstructorsAmbiguous(context, bestDirectives)));
        }
    }
}