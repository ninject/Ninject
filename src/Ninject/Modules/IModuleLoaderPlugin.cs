#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
#endregion
#if !SILVERLIGHT
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
#endif