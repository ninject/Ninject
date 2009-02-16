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
#endregion

namespace Ninject.Planning.Bindings
{
	/// <summary>
	/// Additional information available about a binding, which can be used in constraints
	/// to select bindings to use in activation.
	/// </summary>
	public class BindingMetadata : IBindingMetadata
	{
		private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

		/// <summary>
		/// Gets or sets the binding's name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Determines whether a piece of metadata with the specified key has been defined.
		/// </summary>
		/// <param name="key">The metadata key.</param>
		/// <returns><c>True</c> if such a piece of metadata exists; otherwise, <c>false</c>.</returns>
		public bool Has(string key)
		{
			return _values.ContainsKey(key);
		}

		/// <summary>
		/// Gets the value of metadata defined with the specified key, cast to the specified type.
		/// </summary>
		/// <typeparam name="T">The type of value to expect.</typeparam>
		/// <param name="key">The metadata key.</param>
		/// <returns>The metadata value.</returns>
		public T Get<T>(string key)
		{
			return (T)Get(key);
		}

		/// <summary>
		/// Gets the value of metadata defined with the specified key.
		/// </summary>
		/// <param name="key">The metadata key.</param>
		/// <returns>The metadata value.</returns>
		public object Get(string key)
		{
			return _values.ContainsKey(key) ? _values[key] : null;
		}

		/// <summary>
		/// Sets the value of a piece of metadata.
		/// </summary>
		/// <param name="key">The metadata key.</param>
		/// <param name="value">The metadata value.</param>
		public void Set(string key, object value)
		{
			_values[key] = value;
		}
	}
}