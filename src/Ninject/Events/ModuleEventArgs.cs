#region License
// Author: Nate Kohari <nkohari@gmail.com>
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
using Ninject.Modules;
#endregion

namespace Ninject.Events
{
	/// <summary>
	/// Data related to an event concerning an <see cref="IModule"/>.
	/// </summary>
	public class ModuleEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the module associated with the event.
		/// </summary>
		public IModule Module { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ModuleEventArgs"/> class.
		/// </summary>
		/// <param name="module">The module.</param>
		public ModuleEventArgs(IModule module)
		{
			Module = module;
		}
	}
}