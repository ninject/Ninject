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
#endregion

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// Starts instances that implement <see cref="IStartable"/> during activation,
	/// and stops them during deactivation.
	/// </summary>
	public class StartableStrategy : ActivationStrategyFor<IStartable>
	{
		/// <summary>
		/// Starts the specified instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public override void Activate(IContext context, IStartable instance)
		{
			instance.Start();
		}

		/// <summary>
		/// Stops the specified instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public override void Deactivate(IContext context, IStartable instance)
		{
			instance.Stop();
		}
	}
}