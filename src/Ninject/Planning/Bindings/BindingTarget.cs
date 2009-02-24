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
using Ninject.Activation;
using Ninject.Parameters;
#endregion

namespace Ninject.Planning.Bindings
{
	/// <summary>
	/// Describes the target of a binding.
	/// </summary>
	public enum BindingTarget
	{
		/// <summary>
		/// Indicates that the binding is from a type to itself.
		/// </summary>
		Self,

		/// <summary>
		/// Indicates that the binding is from one type to another.
		/// </summary>
		Type,

		/// <summary>
		/// Indicates that the binding is from a type to a provider.
		/// </summary>
		Provider,

		/// <summary>
		/// Indicates that the binding is from a type to a callback method.
		/// </summary>
		Method,

		/// <summary>
		/// Indicates that the binding is from a type to a constant value.
		/// </summary>
		Constant
	}
}