#region License
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region Using Directives
using System;
#endregion

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// An activation strategy that is only executed if the instance being activated
	/// or deactivated is of a specified type.
	/// </summary>
	/// <typeparam name="T">The type of instance the strategy deals with.</typeparam>
	public abstract class ActivationStrategyFor<T> : ActivationStrategy
	{
		/// <summary>
		/// Contributes to the activation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public sealed override void Activate(IContext context)
		{
			if (context.Instance is T)
				Activate(context, (T)context.Instance);
		}

		/// <summary>
		/// Contributes to the deactivation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public sealed override void Deactivate(IContext context)
		{
			if (context.Instance is T)
				Deactivate(context, (T)context.Instance);
		}

		/// <summary>
		/// Contributes to the activation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public virtual void Activate(IContext context, T instance) { }

		/// <summary>
		/// Contributes to the deactivation of the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="instance">The instance.</param>
		public virtual void Deactivate(IContext context, T instance) { }
	}
}