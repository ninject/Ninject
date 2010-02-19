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
using Ninject.Components;
#endregion

namespace Ninject.Planning.Strategies
{
	/// <summary>
	/// Contributes to the generation of a <see cref="IPlan"/>.
	/// </summary>
	public interface IPlanningStrategy : INinjectComponent
	{
		/// <summary>
		/// Contributes to the specified plan.
		/// </summary>
		/// <param name="plan">The plan that is being generated.</param>
		void Execute(IPlan plan);
	}
}