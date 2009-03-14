#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using Ninject.Components;
#endregion

namespace Ninject.Modules
{
    /// <summary>
    /// A marker for registering a decoratable module loader
    /// </summary>
    public interface IDecoratableModuleLoader : IModuleLoader
    {
        
    }

	/// <summary>
	/// Finds modules defined in external files.
	/// </summary>
	public interface IModuleLoader : INinjectComponent
	{
		/// <summary>
		/// Gets the plugins that will be used to load modules.
		/// </summary>
		ICollection<IModuleLoaderPlugin> Plugins { get; }

		/// <summary>
		/// Loads any modules found in files in the specified path.
		/// </summary>
		/// <param name="path">The path to search.</param>
		void LoadModules(string path);

		/// <summary>
		/// Loads any modules found in files in the specified path.
		/// </summary>
		/// <param name="path">The path to search.</param>
		/// <param name="recursive">If <see langword="true"/>, search the path's subdirectories as well.</param>
		void LoadModules(string path, bool recursive);
	}
}