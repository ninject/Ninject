// -------------------------------------------------------------------------------------------------
// <copyright file="IParameter.cs" company="Ninject Project Contributors">
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

namespace Ninject.Parameters
{
    using System;

    using Ninject.Activation;
    using Ninject.Planning.Targets;

    /// <summary>
    /// Modifies an activation process in some way.
    /// </summary>
    public interface IParameter : IEquatable<IParameter>
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the parameter should be inherited into child requests.
        /// </summary>
        bool ShouldInherit { get; }

        /// <summary>
        /// Gets the value for the parameter within the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="target">The target.</param>
        /// <returns>The value for the parameter.</returns>
        object GetValue(IContext context, ITarget target);
    }
}