#region License
// Author: Nate Kohari <nkohari@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using Ninject.Activation.Strategies;
using Ninject.Components;
using Ninject.Infrastructure.Language;
#endregion

namespace Ninject.Activation
{
	/// <summary>
	/// Drives the activation (injection, etc.) of an instance.
	/// </summary>
	public class Pipeline : NinjectComponent, IPipeline
	{
		/// <summary>
		/// Gets the strategies that contribute to the activation and deactivation processes.
		/// </summary>
		public IList<IActivationStrategy> Strategies { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Pipeline"/> class.
		/// </summary>
		/// <param name="strategies">The strategies to execute during activation and deactivation.</param>
		public Pipeline(IEnumerable<IActivationStrategy> strategies)
		{
			Strategies = strategies.ToList();
		}

		/// <summary>
		/// Activates the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Activate(IContext context)
		{
			Strategies.Map(s => s.Activate(context));
		}

		/// <summary>
		/// Deactivates the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Deactivate(IContext context)
		{
			Strategies.Map(s => s.Deactivate(context));
		}
	}
}