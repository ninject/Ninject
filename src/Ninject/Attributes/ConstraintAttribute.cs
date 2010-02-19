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

namespace Ninject
{
	/// <summary>
	/// Defines a constraint on the decorated member.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
	public abstract class ConstraintAttribute : Attribute
	{
		/// <summary>
		/// Determines whether the specified binding metadata matches the constraint.
		/// </summary>
		/// <param name="metadata">The metadata in question.</param>
		/// <returns><c>True</c> if the metadata matches; otherwise <c>false</c>.</returns>
		public abstract bool Matches(IBindingMetadata metadata);
	}
}
