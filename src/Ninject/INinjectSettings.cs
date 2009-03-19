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
#endregion

namespace Ninject
{
	/// <summary>
	/// Contains configuration options for Ninject.
	/// </summary>
	public interface INinjectSettings
	{
		/// <summary>
		/// Gets the attribute that indicates that a member should be injected.
		/// </summary>
		Type InjectAttribute { get; }

		/// <summary>
		/// Gets the interval at which the cache should be pruned.
		/// </summary>
		TimeSpan CachePruningInterval { get; }

		/// <summary>
		/// Gets a value indicating whether the kernel should automatically load extensions at startup.
		/// </summary>
		bool LoadExtensions { get; }

		/// <summary>
		/// Gets the path that should be searched for extensions.
		/// </summary>
		string ExtensionSearchPattern { get; }

		/// <summary>
		/// Gets a value indicating whether Ninject should use reflection-based injection instead of
		/// the (usually faster) lightweight code generation system.
		/// </summary>
		bool UseReflectionBasedInjection { get; }

		/// <summary>
		/// Gets the value for the specified key.
		/// </summary>
		/// <typeparam name="T">The type of value to return.</typeparam>
		/// <param name="key">The setting's key.</param>
		/// <param name="defaultValue">The value to return if no setting is available.</param>
		/// <returns>The value, or the default value if none was found.</returns>
		T Get<T>(string key, T defaultValue);

		/// <summary>
		/// Sets the value for the specified key.
		/// </summary>
		/// <param name="key">The setting's key.</param>
		/// <param name="value">The setting's value.</param>
		void Set(string key, object value);
	}
}
