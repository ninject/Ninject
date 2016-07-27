//-------------------------------------------------------------------------------------------------
// <copyright file="Pipeline.cs" company="Ninject Project Contributors">
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
namespace Ninject.Activation
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Ninject.Activation.Caching;
    using Ninject.Activation.Strategies;
    using Ninject.Components;
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
            Contract.Requires(strategies != null);
            Contract.Requires(activationCache != null);

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
            Contract.Requires(context != null);
            Contract.Requires(reference != null);

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
            Contract.Requires(context != null);
            Contract.Requires(reference != null);

            if (!this.activationCache.IsDeactivated(reference.Instance))
            {
                this.Strategies.Map(s => s.Deactivate(context, reference));
            }
        }
    }
}