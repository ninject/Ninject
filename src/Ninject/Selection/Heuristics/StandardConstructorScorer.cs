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
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;
#endregion

namespace Ninject.Selection.Heuristics
{
	/// <summary>
	/// Scores constructors by either looking for the existence of an injection marker
	/// attribute, or by counting the number of parameters.
	/// </summary>
	public class StandardConstructorScorer : NinjectComponent, IConstructorScorer
	{
		/// <summary>
		/// Gets the score for the specified constructor.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <returns>The constructor's score.</returns>
		public int Score(ConstructorInfo constructor)
		{
			Ensure.ArgumentNotNull(constructor, "constructor");
			return constructor.HasAttribute(Settings.InjectAttribute) ? Int32.MaxValue : constructor.GetParameters().Length;
		}
	}
}