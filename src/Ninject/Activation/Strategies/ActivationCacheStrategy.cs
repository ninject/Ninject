//-------------------------------------------------------------------------------------------------
// <copyright file="ActivationCacheStrategy.cs" company="Ninject Project Contributors">
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
namespace Ninject.Activation.Strategies
{
    using System.Diagnostics.Contracts;
    using Ninject.Activation.Caching;

    /// <summary>
    /// Adds all activated instances to the activation cache.
    /// </summary>
    public class ActivationCacheStrategy : IActivationStrategy
    {
        /// <summary>
        /// The activation cache.
        /// </summary>
        private readonly IActivationCache activationCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationCacheStrategy"/> class.
        /// </summary>
        /// <param name="activationCache">The activation cache.</param>
        public ActivationCacheStrategy(IActivationCache activationCache)
        {
            Contract.Requires(activationCache != null);
            this.activationCache = activationCache;
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The ninject settings.</value>
        public INinjectSettings Settings { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Contributes to the activation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being activated.</param>
        public void Activate(IContext context, InstanceReference reference)
        {
            this.activationCache.AddActivatedInstance(reference.Instance);
        }

        /// <summary>
        /// Contributes to the deactivation of the instance in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">A reference to the instance being deactivated.</param>
        public void Deactivate(IContext context, InstanceReference reference)
        {
            this.activationCache.AddDeactivatedInstance(reference.Instance);
        }
    }
}