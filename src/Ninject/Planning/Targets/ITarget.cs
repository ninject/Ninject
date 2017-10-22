// -------------------------------------------------------------------------------------------------
// <copyright file="ITarget.cs" company="Ninject Project Contributors">
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

namespace Ninject.Planning.Targets
{
    using System;
    using System.Reflection;

    using Ninject.Activation;
    using Ninject.Planning.Bindings;

    /// <summary>
    /// Represents a site on a type where a value will be injected.
    /// </summary>
    public interface ITarget : ICustomAttributeProvider
    {
        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the name of the target.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the member that contains the target.
        /// </summary>
        MemberInfo Member { get; }

        /// <summary>
        /// Gets the constraint defined on the target.
        /// </summary>
        Func<IBindingMetadata, bool> Constraint { get; }

        /// <summary>
        /// Gets a value indicating whether the target represents an optional dependency.
        /// </summary>
        bool IsOptional { get; }

        /// <summary>
        /// Gets a value indicating whether the target has a default value.
        /// </summary>
        bool HasDefaultValue { get; }

        /// <summary>
        /// Gets the default value for the target.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">If the item does not have a default value.</exception>
        object DefaultValue { get; }

        /// <summary>
        /// Resolves a value for the target within the specified parent context.
        /// </summary>
        /// <param name="parent">The parent context.</param>
        /// <returns>The resolved value.</returns>
        object ResolveWithin(IContext parent);
    }
}