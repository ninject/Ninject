//-------------------------------------------------------------------------------
// <copyright file="BindingConfiguration.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2011 Ninject Project Contributors
//   Authors: Remo Gloor (remo.gloor@gmail.com)
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
//-------------------------------------------------------------------------------

namespace Ninject.Planning.Bindings
{
    using System;
    using System.Collections.Generic;
    using Ninject.Activation;
    using Ninject.Infrastructure;
    using Ninject.Parameters;

    /// <summary>
    /// The configuration of a binding.
    /// </summary>
    public class BindingConfiguration : IBindingConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingConfiguration"/> class.
        /// </summary>
        public BindingConfiguration()
        {
            this.Metadata = new BindingMetadata();
            this.Parameters = new List<IParameter>();
            this.ActivationActions = new List<Action<IContext, object>>();
            this.DeactivationActions = new List<Action<IContext, object>>();
            this.ScopeCallback = StandardScopeCallbacks.Transient;
        }

        /// <summary>
        /// Gets the binding's metadata.
        /// </summary>
        public IBindingMetadata Metadata { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the binding was implicitly registered.
        /// </summary>
        public bool IsImplicit { get; set; }

        /// <summary>
        /// Gets a value indicating whether the binding has a condition associated with it.
        /// </summary>
        public bool IsConditional
        {
            get { return this.Condition != null; }
        }

        /// <summary>
        /// Gets or sets the type of target for the binding.
        /// </summary>
        public BindingTarget Target { get; set; }

        /// <summary>
        /// Gets or sets the condition defined for the binding.
        /// </summary>
        public Func<IRequest, bool> Condition { get; set; }

        /// <summary>
        /// Gets or sets the callback that returns the provider that should be used by the binding.
        /// </summary>
        public Func<IContext, IProvider> ProviderCallback { get; set; }

        /// <summary>
        /// Gets or sets the callback that returns the object that will act as the binding's scope.
        /// </summary>
        public Func<IContext, object> ScopeCallback { get; set; }

        /// <summary>
        /// Gets the parameters defined for the binding.
        /// </summary>
        public ICollection<IParameter> Parameters { get; private set; }

        /// <summary>
        /// Gets the actions that should be called after instances are activated via the binding.
        /// </summary>
        public ICollection<Action<IContext, object>> ActivationActions { get; private set; }

        /// <summary>
        /// Gets the actions that should be called before instances are deactivated via the binding.
        /// </summary>
        public ICollection<Action<IContext, object>> DeactivationActions { get; private set; }

        /// <summary>
        /// Gets the provider for the binding.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The provider to use.</returns>
        public IProvider GetProvider(IContext context)
        {
            Ensure.ArgumentNotNull(context, "context");
            return this.ProviderCallback(context);
        }

        /// <summary>
        /// Gets the scope for the binding, if any.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The object that will act as the scope, or <see langword="null"/> if the service is transient.</returns>
        public object GetScope(IContext context)
        {
            Ensure.ArgumentNotNull(context, "context");
            return this.ScopeCallback(context);
        }

        /// <summary>
        /// Determines whether the specified request satisfies the conditions defined on this binding.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the request satisfies the conditions; otherwise <c>false</c>.</returns>
        public bool Matches(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");
            return this.Condition == null || this.Condition(request);
        }    
    }
}