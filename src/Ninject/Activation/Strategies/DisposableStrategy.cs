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
	/// During deactivation, disposes instances that implement <see cref="IDisposable"/>.
	/// </summary>
	public class DisposableStrategy : ActivationStrategyFor<IDisposable>
	{
		/// <summary>
		/// Disposes the specified instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public override void Deactivate(IContext context, IDisposable instance)
		{
			instance.Dispose();
		}
	}
}