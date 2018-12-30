﻿// -------------------------------------------------------------------------------------------------
// <copyright file="SelfBindingResolver.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Bindings.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Activation;
    using Ninject.Activation.Providers;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Selection.Heuristics;

    /// <summary>
    /// Represents a binding resolver that use the service in question itself as the target to activate.
    /// </summary>
    public class SelfBindingResolver : NinjectComponent, IMissingBindingResolver
    {
        private readonly IPlanner planner;
        private readonly IConstructorScorer constructorScorer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfBindingResolver"/> class.
        /// </summary>
        /// <param name="planner">The <see cref="IPlanner"/> component.</param>
        /// <param name="constructorScorer">The <see cref="IConstructorScorer"/> component.</param>
        public SelfBindingResolver(IPlanner planner, IConstructorScorer constructorScorer)
        {
            Ensure.ArgumentNotNull(planner, nameof(planner));
            Ensure.ArgumentNotNull(constructorScorer, nameof(constructorScorer));

            this.planner = planner;
            this.constructorScorer = constructorScorer;
        }

        /// <summary>
        /// Returns any bindings from the specified collection that match the specified service.
        /// </summary>
        /// <param name="bindings">The dictionary of all registered bindings.</param>
        /// <param name="request">The service in question.</param>
        /// <returns>
        /// The series of matching bindings.
        /// </returns>
        public IEnumerable<IBinding> Resolve(IDictionary<Type, ICollection<IBinding>> bindings, IRequest request)
        {
            var service = request.Service;

            if (!this.TypeIsSelfBindable(service))
            {
                return Enumerable.Empty<IBinding>();
            }

            return new[]
            {
                new Binding(service)
                {
                    ProviderCallback = ctx => new StandardProvider(service, this.planner, this.constructorScorer),
                },
            };
        }

        /// <summary>
        /// Returns a value indicating whether the specified service is self-bindable.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>
        /// <see langword="true"/> if the type is self-bindable; otherwise, <see langword="false"/>.
        /// </returns>
        protected virtual bool TypeIsSelfBindable(Type service)
        {
            return !service.IsInterface
                   && !service.IsAbstract
                   && !service.IsValueType
                   && service != typeof(string)
                   && !service.ContainsGenericParameters;
        }
    }
}