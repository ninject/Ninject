// -------------------------------------------------------------------------------------------------
// <copyright file="Pipeline.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation
{
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Language;

    /// <summary>
    /// Drives the activation (injection, etc.) of an instance.
    /// </summary>
    public class Pipeline : NinjectComponent, IPipeline
    {
        /// <summary>
        /// The activation cache.
        /// </summary>
        private readonly IActivationCache activationCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pipeline"/> class.
        /// </summary>
        /// <param name="strategies">The strategies to execute during activation and deactivation.</param>
        /// <param name="activationCache">The activation cache.</param>
        public Pipeline(IEnumerable<IActivationStrategy> strategies, IActivationCache activationCache)
        {
            Ensure.ArgumentNotNull(strategies, "strategies");
            Ensure.ArgumentNotNull(activationCache, "activationCache");

            this.Strategies = strategies.ToList();
            this.activationCache = activationCache;
        }

        /// <summary>
        /// Gets the strategies that contribute to the activation and deactivation processes.
        /// </summary>
        public IList<IActivationStrategy> Strategies { get; private set; }

        /// <summary>
        /// Activates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Activate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(reference, "reference");

            if (!this.activationCache.IsActivated(reference.Instance))
            {
                this.Strategies.Map(s => s.Activate(context, reference));
            }
        }

        /// <summary>
        /// Deactivates the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The instance reference.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            Ensure.ArgumentNotNull(context, "context");
            Ensure.ArgumentNotNull(reference, "reference");

            if (!this.activationCache.IsDeactivated(reference.Instance))
            {
                this.Strategies.Map(s => s.Deactivate(context, reference));
            }
        }
    }
}