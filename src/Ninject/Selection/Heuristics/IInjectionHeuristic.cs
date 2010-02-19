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
using System.Reflection;
using Ninject.Components;
#endregion

namespace Ninject.Selection.Heuristics
{
	/// <summary>
	/// Determines whether members should be injected during activation.
	/// </summary>
	public interface IInjectionHeuristic : INinjectComponent
	{
		/// <summary>
		/// Returns a value indicating whether the specified member should be injected.
		/// </summary>
		/// <param name="member">The member in question.</param>
		/// <returns><c>True</c> if the member should be injected; otherwise <c>false</c>.</returns>
		bool ShouldInject(MemberInfo member);
	}
}