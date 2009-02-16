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
using System.Linq;
using Ninject.Injection;
using Ninject.Planning.Directives;
#endregion

namespace Ninject.Activation.Strategies
{
	/// <summary>
	/// Injects methods on an instance during activation.
	/// </summary>
	public class MethodInjectionStrategy : ActivationStrategy
	{
		/// <summary>
		/// Gets the injector factory component.
		/// </summary>
		public IInjectorFactory InjectorFactory { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjectionStrategy"/> class.
		/// </summary>
		/// <param name="injectorFactory">The injector factory component.</param>
		public MethodInjectionStrategy(IInjectorFactory injectorFactory)
		{
			InjectorFactory = injectorFactory;
		}

		/// <summary>
		/// Injects values into the properties as described by <see cref="MethodInjectionDirective"/>s
		/// contained in the plan.
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Activate(IContext context)
		{
			foreach (var directive in context.Plan.GetAll<MethodInjectionDirective>())
			{
				var injector = InjectorFactory.GetMethodInjector(directive.Member);
				var arguments = directive.Targets.Select(target => target.ResolveWithin(context));
				injector.Invoke(context.Instance, arguments.ToArray());
			}
		}
	}
}