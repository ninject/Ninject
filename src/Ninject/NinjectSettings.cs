#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
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

		#if !NO_ASSEMBLY_SCANNING
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
		#endif //!NO_ASSEMBLY_SCANNING

		#if !NO_LCG
		/// <summary>
		/// Gets a value indicating whether Ninject should use reflection-based injection instead of
		/// the (usually faster) lightweight code generation system.
		/// </summary>
		public bool UseReflectionBasedInjection
		{
			get { return Get("UseReflectionBasedInjection", false); }
			set { Set("UseReflectionBasedInjection", value); }
		}
		#endif //!NO_LCG

		#if !SILVERLIGHT
		/// <summary>
		/// Gets a value indicating whether Ninject should inject non public members.
		/// </summary>
		public bool InjectNonPublic
		{
			get { return Get("InjectNonPublic", false); }
			set { Set("InjectNonPublic", value); }
		}
		#endif //!SILVERLIGHT

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
