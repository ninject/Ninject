// -------------------------------------------------------------------------------------------------
// <copyright file="INinjectModule.cs" company="Ninject Project Contributors">
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

namespace Ninject.Modules
{
    using Ninject.Infrastructure;

    /// <summary>
    /// A pluggable unit that can be loaded into an <see cref="IKernel"/>.
    /// </summary>
    public interface INinjectModule : IHaveKernel
    {
        /// <summary>
        /// Gets the module's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Called when the module is loaded into a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is loading the module.</param>
        void OnLoad(IKernel kernel);

        /// <summary>
        /// Called when the module is unloaded from a kernel.
        /// </summary>
        /// <param name="kernel">The kernel that is unloading the module.</param>
        void OnUnload(IKernel kernel);

        /// <summary>
        /// Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        void OnVerifyRequiredModules();
    }
}