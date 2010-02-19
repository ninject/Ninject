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
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Activation
{
	/// <summary>
	/// Holds an instance during activation or after it has been cached.
	/// </summary>
	public class InstanceReference
	{
		/// <summary>
		/// Gets or sets the instance.
		/// </summary>
		public object Instance { get; set; }

		/// <summary>
		/// Returns a value indicating whether the instance is of the specified type.
		/// </summary>
		/// <typeparam name="T">The type in question.</typeparam>
		/// <returns><see langword="True"/> if the instance is of the specified type, otherwise <see langword="false"/>.</returns>
		public bool Is<T>()
		{
			return Instance is T;
		}

		/// <summary>
		/// Returns the instance as the specified type.
		/// </summary>
		/// <typeparam name="T">The requested type.</typeparam>
		/// <returns>The instance.</returns>
		public T As<T>()
		{
			return (T)Instance;
		}

		/// <summary>
		/// Executes the specified action if the instance if of the specified type.
		/// </summary>
		/// <typeparam name="T">The type in question.</typeparam>
		/// <param name="action">The action to execute.</param>
		public void IfInstanceIs<T>(Action<T> action)
		{
			if (Instance is T)
				action((T)Instance);
		}
	}
}