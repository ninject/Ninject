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

namespace Ninject.Activation.Providers
{
	/// <summary>
	/// A provider that always returns the same constant value.
	/// </summary>
	/// <typeparam name="T">The type of value that is returned.</typeparam>
	public class ConstantProvider<T> : Provider<T>
	{
		/// <summary>
		/// Gets the value that the provider will return.
		/// </summary>
		public T Value { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstantProvider&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="value">The value that the provider should return.</param>
		public ConstantProvider(T value)
		{
			Value = value;
		}

		/// <summary>
		/// Creates an instance within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The constant value this provider returns.</returns>
		protected override T CreateInstance(IContext context)
		{
			return Value;
		}
	}
}