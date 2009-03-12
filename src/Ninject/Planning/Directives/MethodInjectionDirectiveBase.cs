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
using System.Reflection;
using Ninject.Infrastructure;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a method or constructor.
	/// </summary>
	public abstract class MethodInjectionDirectiveBase<TMethod, TInjector> : IDirective
		where TMethod : MethodBase
	{
		/// <summary>
		/// Gets or sets the injector that will be triggered.
		/// </summary>
		public TInjector Injector { get; private set; }

		/// <summary>
		/// Gets or sets the targets for the directive.
		/// </summary>
		public ITarget[] Targets { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjectionDirectiveBase&lt;TMethod, TInjector&gt;"/> class.
		/// </summary>
		/// <param name="method">The method this directive represents.</param>
		/// <param name="injector">The injector that will be triggered.</param>
		protected MethodInjectionDirectiveBase(TMethod method, TInjector injector)
		{
			Ensure.ArgumentNotNull(method, "method");
			Ensure.ArgumentNotNull(injector, "injector");

			Injector = injector;
			Targets = CreateTargetsFromParameters(method);
		}

		/// <summary>
		/// Creates targets for the parameters of the method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The targets for the method's parameters.</returns>
		protected virtual ITarget[] CreateTargetsFromParameters(TMethod method)
		{
			return method.GetParameters().Select(parameter => new ParameterTarget(method, parameter)).ToArray();
		}
	}
}
