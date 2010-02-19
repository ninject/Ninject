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
	/// Modifies an activation process in some way.
	/// </summary>
	public interface IParameter : IEquatable<IParameter>
	{
		/// <summary>
		/// Gets the name of the parameter.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a value indicating whether the parameter should be inherited into child requests.
		/// </summary>
		bool ShouldInherit { get; }

		/// <summary>
		/// Gets the value for the parameter within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The value for the parameter.</returns>
		object GetValue(IContext context);
	}
}