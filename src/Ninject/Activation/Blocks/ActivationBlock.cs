// -------------------------------------------------------------------------------------------------
// <copyright file="ActivationBlock.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Blocks
{
    using System;
    using System.Collections.Generic;

    using Ninject.Infrastructure;
    using Ninject.Infrastructure.Disposal;
    using Ninject.Parameters;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    /// <summary>
    /// A block used for deterministic disposal of activated instances. When the block is
    /// disposed, all instances activated via it will be deactivated.
    /// </summary>
    public class ActivationBlock : DisposableObject, IActivationBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationBlock"/> class.
        /// </summary>
        /// <param name="parent">The parent resolution root.</param>
        public ActivationBlock(IResolutionRoot parent)
        {
            Ensure.ArgumentNotNull(parent, "parent");

            this.Parent = parent;
        }

        /// <summary>
        /// Gets the parent resolution root (usually the kernel).
        /// </summary>
        public IResolutionRoot Parent { get; private set; }

        /// <summary>
        /// Injects the specified existing instance, without managing its lifecycle.
        /// </summary>
        /// <param name="instance">The instance to inject.</param>
        /// <param name="parameters">The parameters to pass to the request.</param>
        public void Inject(object instance, params IParameter[] parameters)
        {
            this.Parent.Inject(instance, parameters);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns><c>True</c> if the request can be resolved; otherwise, <c>false</c>.</returns>
        public bool CanResolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

            return this.Parent.CanResolve(request);
        }

        /// <summary>
        /// Determines whether the specified request can be resolved.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="ignoreImplicitBindings">if set to <c>true</c> implicit bindings are ignored.</param>
        /// <returns>
        ///     <c>True</c> if the request can be resolved; otherwise, <c>false</c>.
        /// </returns>
        public bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            Ensure.ArgumentNotNull(request, "request");

            return this.Parent.CanResolve(request, ignoreImplicitBindings);
        }

        /// <summary>
        /// Resolves instances for the specified request. The instances are not actually resolved
        /// until a consumer iterates over the enumerator.
        /// </summary>
        /// <param name="request">The request to resolve.</param>
        /// <returns>An enumerator of instances that match the request.</returns>
        public IEnumerable<object> Resolve(IRequest request)
        {
            Ensure.ArgumentNotNull(request, "request");

            return this.Parent.Resolve(request);
        }

        /// <summary>
        /// Creates a request for the specified service.
        /// </summary>
        /// <param name="service">The service that is being requested.</param>
        /// <param name="constraint">The constraint to apply to the bindings to determine if they match the request.</param>
        /// <param name="parameters">The parameters to pass to the resolution.</param>
        /// <param name="isOptional"><c>True</c> if the request is optional; otherwise, <c>false</c>.</param>
        /// <param name="isUnique"><c>True</c> if the request should return a unique result; otherwise, <c>false</c>.</param>
        /// <returns>The created request.</returns>
        public virtual IRequest CreateRequest(Type service, Func<IBindingMetadata, bool> constraint, IEnumerable<IParameter> parameters, bool isOptional, bool isUnique)
        {
            Ensure.ArgumentNotNull(service, "service");
            Ensure.ArgumentNotNull(parameters, "parameters");

            return new Request(service, constraint, parameters, () => this, isOptional, isUnique);
        }

        /// <summary>
        /// Deactivates and releases the specified instance if it is currently managed by Ninject.
        /// </summary>
        /// <param name="instance">The instance to release.</param>
        /// <returns><see langword="True"/> if the instance was found and released; otherwise <see langword="false"/>.</returns>
        public bool Release(object instance)
        {
            return this.Parent.Release(instance);
        }
    }
}