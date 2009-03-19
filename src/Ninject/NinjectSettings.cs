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

namespace Ninject
{
	/// <summary>
	/// Contains configuration options for Ninject.
	/// </summary>
	public class NinjectSettings : INinjectSettings
	{
		private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

		/// <summary>
		/// Gets or sets the attribute that indicates that a member should be injected.
		/// </summary>
		public Type InjectAttribute
		{
			get { return Get("InjectAttribute", typeof(InjectAttribute)); }
			set { Set("InjectAttribute", value); }
		}

		/// <summary>
		/// Gets or sets the interval at which the GC should be polled.
		/// </summary>
		public TimeSpan CachePruningInterval
		{
			get { return Get("CachePruningInterval", TimeSpan.FromSeconds(30)); }
			set { Set("CachePruningInterval", value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the kernel should automatically load extensions at startup.
		/// </summary>
		public bool LoadExtensions
		{
			get { return Get("LoadExtensions", true); }
			set { Set("LoadExtensions", value); }
		}

		/// <summary>
		/// Gets or sets the path that should be searched for extensions.
		/// </summary>
		public string ExtensionSearchPattern
		{
			get { return Get("ExtensionSearchPattern", "Ninject.Extensions.*.dll"); }
			set { Set("ExtensionSearchPattern", value); }
		}

		/// <summary>
		/// Gets a value indicating whether Ninject should use reflection-based injection instead of
		/// the (usually faster) lightweight code generation system.
		/// </summary>
		public bool UseReflectionBasedInjection
		{
			get { return Get("UseReflectionBasedInjection", false); }
			set { Set("UseReflectionBasedInjection", value); }
		}

		/// <summary>
		/// Gets the value for the specified key.
		/// </summary>
		/// <typeparam name="T">The type of value to return.</typeparam>
		/// <param name="key">The setting's key.</param>
		/// <param name="defaultValue">The value to return if no setting is available.</param>
		/// <returns>The value, or the default value if none was found.</returns>
		public T Get<T>(string key, T defaultValue)
		{
			return _values.ContainsKey(key) ? (T)_values[key] : defaultValue;
		}

		/// <summary>
		/// Sets the value for the specified key.
		/// </summary>
		/// <param name="key">The setting's key.</param>
		/// <param name="value">The setting's value.</param>
		public void Set(string key, object value)
		{
			_values[key] = value;
		}
	}
}
