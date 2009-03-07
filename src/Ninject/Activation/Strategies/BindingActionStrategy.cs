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
using Ninject.Infrastructure;
using Ninject.Infrastructure.Language;
#endregion

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// Executes actions defined on the binding during activation and deactivation.
	/// </summary>
	public class BindingActionStrategy : ActivationStrategy
	{
		/// <summary>
		/// Calls the activation actions defined on the binding.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Activate(IContext context)
		{
			Ensure.ArgumentNotNull(context, "context");
			context.Binding.ActivationActions.Map(action => action(context));
		}

		/// <summary>
		/// Calls the deactivation actions defined on the binding.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Deactivate(IContext context)
		{
			Ensure.ArgumentNotNull(context, "context");
			context.Binding.DeactivationActions.Map(action => action(context));
		}
	}
}