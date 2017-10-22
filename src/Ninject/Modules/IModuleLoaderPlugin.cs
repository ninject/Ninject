// -------------------------------------------------------------------------------------------------
// <copyright file="IModuleLoaderPlugin.cs" company="Ninject Project Contributors">
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
    using System.Collections.Generic;

    using Ninject.Components;

    /// <summary>
    /// Loads modules at runtime by searching external files.
    /// </summary>
    public interface IModuleLoaderPlugin : INinjectComponent
    {
        /// <summary>
        /// Gets the file extensions that the plugin understands how to load.
        /// </summary>
        IEnumerable<string> SupportedExtensions { get; }

        /// <summary>
        /// Loads modules from the specified files.
        /// </summary>
        /// <param name="filenames">The names of the files to load modules from.</param>
        void LoadModules(IEnumerable<string> filenames);
    }
}