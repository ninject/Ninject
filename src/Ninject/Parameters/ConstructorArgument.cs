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
using Ninject.Activation;
#endregion

namespace Ninject.Parameters
{
	/// <summary>
	/// Overrides the injected value of a constructor argument.
	/// </summary>
	public class ConstructorArgument : Parameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
		/// </summary>
		/// <param name="name">The name of the argument to override.</param>
		/// <param name="value">The value to inject into the property.</param>
		public ConstructorArgument(string name, object value) : base(name, value, false) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstructorArgument"/> class.
		/// </summary>
		/// <param name="name">The name of the argument to override.</param>
		/// <param name="valueCallback">The callback to invoke to get the value that should be injected.</param>
		public ConstructorArgument(string name, Func<IContext, object> valueCallback) : base(name, valueCallback, false) { }
	}
}