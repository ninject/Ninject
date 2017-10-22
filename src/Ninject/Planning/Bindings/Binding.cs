// -------------------------------------------------------------------------------------------------
// <copyright file="Binding.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Bindings
{
    using System;
    using System.Collections.Generic;

    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Parameters;

    /// <summary>
    /// Contains information about a service registration.
    /// </summary>
    public class Binding : IBinding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class.
        /// </summary>
        /// <param name="service">The service that is controlled by the binding.</param>
        public Binding(Type service)
        {
            Ensure.ArgumentNotNull(service, "service");

            this.Service = service;
            this.BindingConfiguration = new BindingConfiguration();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class.
        /// </summary>
        /// <param name="service">The service that is controlled by the binding.</param>
        /// <param name="configuration">The binding configuration.</param>
        public Binding(Type service, IBindingConfiguration configuration)
        {
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(configuration, "configuration");

            this.Service = service;
            this.BindingConfiguration = configuration;
        }

        /// <summary>
        /// Gets the binding configuration.
        /// </summary>
        /// <value>The binding configuration.</value>
        public IBindingConfiguration BindingConfiguration { get; private set; }

        /// <summary>
        /// Gets the service type that is controlled by the binding.
        /// </summary>
        public Type Service { get; private set; }

        /// <summary>
        /// Gets the binding's metadata.
        /// </summary>
        public IBindingMetadata Metadata
        {
            get
            {
                return this.BindingConfiguration.Metadata;
            }
        }

        /// <summary>
        /// Gets or sets the type of target for the binding.
        /// </summary>
        public BindingTarget Target
        {
            get
            {
                return this.BindingConfiguration.Target;
            }

            set
            {
                this.BindingConfiguration.Target = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the binding was implicitly registered.
        /// </summary>
        public bool IsImplicit
        {
            get
            {
                return this.BindingConfiguration.IsImplicit;
            }

            set
            {
                this.BindingConfiguration.IsImplicit = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the binding has a condition associated with it.
        /// </summary>
        public bool IsConditional
        {
            get
            {
                return this.BindingConfiguration.IsConditional;
            }
        }

        /// <summary>
        /// Gets or sets the condition defined for the binding.
        /// </summary>
        public Func<IRequest, bool> Condition
        {
            get
            {
                return this.BindingConfiguration.Condition;
            }

            set
            {
                this.BindingConfiguration.Condition = value;
            }
        }

        /// <summary>
        /// Gets or sets the callback that returns the provider that should be used by the binding.
        /// </summary>
        public Func<IContext, IProvider> ProviderCallback
        {
            get
            {
                return this.BindingConfiguration.ProviderCallback;
            }

            set
            {
                this.BindingConfiguration.ProviderCallback = value;
            }
        }

        /// <summary>
        /// Gets or sets the callback that returns the object that will act as the binding's scope.
        /// </summary>
        public Func<IContext, object> ScopeCallback
        {
            get
            {
                return this.BindingConfiguration.ScopeCallback;
            }

            set
            {
                this.BindingConfiguration.ScopeCallback = value;
            }
        }

        /// <summary>
        /// Gets the parameters defined for the binding.
        /// </summary>
        public ICollection<IParameter> Parameters
        {
            get
            {
                return this.BindingConfiguration.Parameters;
            }
        }

        /// <summary>
        /// Gets the actions that should be called after instances are activated via the binding.
        /// </summary>
        public ICollection<Action<IContext, object>> ActivationActions
        {
            get
            {
                return this.BindingConfiguration.ActivationActions;
            }
        }

        /// <summary>
        /// Gets the actions that should be called before instances are deactivated via the binding.
        /// </summary>
        public ICollection<Action<IContext, object>> DeactivationActions
        {
            get
            {
                return this.BindingConfiguration.DeactivationActions;
            }
        }

        /// <summary>
        /// Gets the provider for the binding.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The provider to use.</returns>
        public IProvider GetProvider(IContext context)
        {
            return this.BindingConfiguration.GetProvider(context);
        }

        /// <summary>
        /// Gets the scope for the binding, if any.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The object that will act as the scope, or <see langword="null"/> if the service is transient.
        /// </returns>
        public object GetScope(IContext context)
        {
            return this.BindingConfiguration.GetScope(context);
        }

        /// <summary>
        /// Determines whether the specified request satisfies the condition defined on the binding,
        /// if one was defined.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>True</c> if the request satisfies the condition; otherwise <c>false</c>.
        /// </returns>
        public bool Matches(IRequest request)
        {
            return this.BindingConfiguration.Matches(request);
        }
    }
}