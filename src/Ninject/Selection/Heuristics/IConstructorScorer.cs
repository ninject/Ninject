#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
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
	/// Generates scores for constructors, to determine which is the best one to call during activation.
	/// </summary>
	public interface IConstructorScorer : INinjectComponent
	{
		/// <summary>
		/// Gets the score for the specified constructor.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <returns>The constructor's score.</returns>
		int Score(ConstructorInfo constructor);
	}
}