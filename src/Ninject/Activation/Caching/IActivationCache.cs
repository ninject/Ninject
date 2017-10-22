// -------------------------------------------------------------------------------------------------
// <copyright file="IActivationCache.cs" company="Ninject Project Contributors">
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

namespace Ninject.Activation.Caching
{
    using Ninject.Components;

    /// <summary>
    /// Stores the objects that were activated
    /// </summary>
    public interface IActivationCache : INinjectComponent
    {
        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds an activated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        void AddActivatedInstance(object instance);

        /// <summary>
        /// Adds an deactivated instance.
        /// </summary>
        /// <param name="instance">The instance to be added.</param>
        void AddDeactivatedInstance(object instance);

        /// <summary>
        /// Determines whether the specified instance is activated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is activated; otherwise, <c>false</c>.
        /// </returns>
        bool IsActivated(object instance);

        /// <summary>
        /// Determines whether the specified instance is deactivated.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified instance is deactivated; otherwise, <c>false</c>.
        /// </returns>
        bool IsDeactivated(object instance);
    }
}