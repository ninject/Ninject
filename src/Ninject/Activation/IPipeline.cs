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
		void Activate(IContext context);

		/// <summary>
		/// Deactivates the instance in the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		void Deactivate(IContext context);
	}
}