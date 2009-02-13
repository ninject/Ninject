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
#endregion

namespace Ninject.Activation.Hooks
{
	/// <summary>
	/// A hook that always returns a constant value.
	/// </summary>
	public class ConstantHook : IHook
	{
		/// <summary>
		/// Gets the value that the hook will return.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstantHook"/> class.
		/// </summary>
		/// <param name="value">The value that the hook should return.</param>
		public ConstantHook(object value)
		{
			Value = value;
		}

		/// <summary>
		/// Resolves the instance associated with this hook.
		/// </summary>
		/// <returns>The resolved instance.</returns>
		public object Resolve()
		{
			return Value;
		}
	}
}