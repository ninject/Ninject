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