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
using System.Collections.Generic;
using Ninject.Activation.Strategies;
using Ninject.Components;
#endregion

namespace Ninject.Activation
{
	/// <summary>
	/// Drives the activation (injection, etc.) of an instance.
	/// </summary>
	public interface IPipeline : INinjectComponent
	{
		/// <summary>
		/// Gets the strategies that contribute to the activation and deactivation processes.
		/// </summary>
		IList<IActivationStrategy> Strategies { get; }

		/// <summary>
		/// Activates the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="reference">The instance reference.</param>
		void Activate(IContext context, InstanceReference reference);

		/// <summary>
		/// Deactivates the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="reference">The instance reference.</param>
		void Deactivate(IContext context, InstanceReference reference);
	}
}