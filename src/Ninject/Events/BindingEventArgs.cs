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
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Events
{
	/// <summary>
	/// Data related to an event concerning an <see cref="IBinding"/>.
	/// </summary>
	public class BindingEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the binding associated with the event.
		/// </summary>
		public IBinding Binding { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BindingEventArgs"/> class.
		/// </summary>
		/// <param name="binding">The binding.</param>
		public BindingEventArgs(IBinding binding)
		{
			Ensure.ArgumentNotNull(binding, "binding");
			Binding = binding;
		}
	}
}