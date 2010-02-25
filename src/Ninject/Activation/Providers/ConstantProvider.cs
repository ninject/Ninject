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
using Ninject.Infrastructure;
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
        /// Initializes a new instance of the ConstantProvider&lt;T&gt; class.
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