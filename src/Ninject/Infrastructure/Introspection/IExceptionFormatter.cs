// -------------------------------------------------------------------------------------------------
// <copyright file="IExceptionFormatter.cs" company="Ninject Project Contributors">
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

namespace Ninject.Infrastructure.Introspection
{
    using System;

    using Ninject.Activation;
    using Ninject.Components;

    /// <summary>
    /// Provides meaningful exception messages.
    /// </summary>
    public interface IExceptionFormatter : INinjectComponent
    {
        /// <summary>
        /// Generates a message saying that the binding could not be resolved on the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The exception message.</returns>
        string CouldNotResolveBinding(IRequest request);

        /// <summary>
        /// Generates a message saying that the specified context has cyclic dependencies.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The exception message.</returns>
        string CyclicalDependenciesDetected(IContext context);

        /// <summary>
        /// Generates a message saying that no constructors are available for the given component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="implementation">The implementation.</param>
        /// <returns>The exception message.</returns>
        string NoConstructorsAvailableForComponent(Type component, Type implementation);

        /// <summary>
        /// Generates a message saying that the specified component is not registered.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>The exception message.</returns>
        string NoSuchComponentRegistered(Type component);

        /// <summary>
        /// Generates a message saying that the provider on the specified context returned null.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The exception message.</returns>
        string ProviderReturnedNull(IContext context);
    }
}