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
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Infrastructure
{
	/// <summary>
	/// Represents a future value.
	/// </summary>
	/// <typeparam name="T">The type of value.</typeparam>
	public class Future<T>
	{
		private bool _hasValue;
		private T _value;

		/// <summary>
		/// Gets the value, resolving it if necessary.
		/// </summary>
		public T Value
		{
			get
			{
				if (!_hasValue)
				{
					_value = Callback();
					_hasValue = true;
				}

				return _value;
			}
		}

		/// <summary>
		/// Gets the callback that will be called to resolve the value.
		/// </summary>
		public Func<T> Callback { get; private set; }

		/// <summary>
        /// Initializes a new instance of the Future&lt;T&gt; class.
		/// </summary>
		/// <param name="callback">The callback that will be triggered to read the value.</param>
		public Future(Func<T> callback)
		{
			Callback = callback;
		}

		/// <summary>
		/// Gets the value from the future.
		/// </summary>
		/// <param name="future">The future.</param>
		/// <returns>The future value.</returns>
		public static implicit operator T(Future<T> future)
		{
			return future.Value;
		}
	}
}