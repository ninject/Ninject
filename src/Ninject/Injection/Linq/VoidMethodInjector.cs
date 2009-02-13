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
using System.Reflection;
#endregion

namespace Ninject.Injection.Linq
{
	/// <summary>
	/// A method injector that injects methods that return <see type="void"/>.
	/// </summary>
	public class VoidMethodInjector : MethodInjectorBase<Action<object, object[]>>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VoidMethodInjector"/> class.
		/// </summary>
		/// <param name="method">The method that will be injected.</param>
		public VoidMethodInjector(MethodInfo method) : base(method) { }

		/// <summary>
		/// Calls the associated method, injecting the specified values.
		/// </summary>
		/// <param name="target">The target object on which to call the method.</param>
		/// <param name="values">The values to inject.</param>
		/// <returns>The return value of the method, or <see langword="null"/> if the method returns <see langword="void"/>.</returns>
		public override object Invoke(object target, object[] values)
		{
			Callback.Invoke(target, values);
			return null;
		}
	}
}