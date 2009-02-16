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
using System.Reflection;
#endregion

namespace Ninject.Injection.Linq
{
	/// <summary>
	/// An injector that injects values into methods.
	/// </summary>
	public class MethodInjector : MethodInjectorBase<Func<object, object[], object>>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjector"/> class.
		/// </summary>
		/// <param name="method">The method that will be injected.</param>
		public MethodInjector(MethodInfo method) : base(method) { }

		/// <summary>
		/// Calls the associated method, injecting the specified values.
		/// </summary>
		/// <param name="target">The target object on which to call the method.</param>
		/// <param name="values">The values to inject.</param>
		/// <returns>The return value of the method, or <see langword="null"/> if the method returns <see type="void"/>.</returns>
		public override object Invoke(object target, object[] values)
		{
			return Callback.Invoke(target, values);
		}
	}
}