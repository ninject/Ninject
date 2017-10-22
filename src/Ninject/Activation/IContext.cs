// -------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Ninject Project Contributors">
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
    using System;
    using System.Collections.Generic;

    using Ninject.Activation.Caching;
    using Ninject.Parameters;
    using Ninject.Planning;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// Contains information about the activation of a single instance.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Gets the kernel that is driving the activation.
        /// </summary>
        IKernel Kernel { get; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        IRequest Request { get; }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        IBinding Binding { get; }

        /// <summary>
        /// Gets or sets the activation plan.
        /// </summary>
        IPlan Plan { get; set; }

        /// <summary>
        /// Gets the cache component.
        /// </summary>
        ICache Cache { get; }

        /// <summary>
        /// Gets the parameters that were passed to manipulate the activation process.
        /// </summary>
        ICollection<IParameter> Parameters { get; }

        /// <summary>
        /// Gets the generic arguments for the request, if any.
        /// </summary>
        Type[] GenericArguments { get; }

        /// <summary>
        /// Gets a value indicating whether the request involves inferred generic arguments.
        /// </summary>
        bool HasInferredGenericArguments { get; }

        /// <summary>
        /// Gets the provider that should be used to create the instance for this context.
        /// </summary>
        /// <returns>The provider that should be used.</returns>
        IProvider GetProvider();

        /// <summary>
        /// Gets the scope for the context that "owns" the instance activated therein.
        /// </summary>
        /// <returns>The object that acts as the scope.</returns>
        object GetScope();

        /// <summary>
        /// Resolves this instance for this context.
        /// </summary>
        /// <returns>The resolved instance.</returns>
        object Resolve();
    }
}