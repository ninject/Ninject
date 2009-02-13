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
using System.Linq;
using System.Reflection;
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a method or constructor.
	/// </summary>
	/// <typeparam name="T">The type of member that the directive describes.</typeparam>
	public abstract class MethodInjectionDirectiveBase<T> : IDirective
		where T : MethodBase
	{
		/// <summary>
		/// Gets the member associated with the directive.
		/// </summary>
		public T Member { get; private set; }

		/// <summary>
		/// Gets the targets for the directive.
		/// </summary>
		public ITarget[] Targets { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodInjectionDirectiveBase{T}"/> class.
		/// </summary>
		/// <param name="member">The method described by the directive.</param>
		protected MethodInjectionDirectiveBase(T member)
		{
			Member = member;
			Targets = GetParameterTargets(member);
		}

		/// <summary>
		/// Creates targets for the parameters of the method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>The targets for the method's parameters.</returns>
		protected ITarget[] GetParameterTargets(T method)
		{
			return method.GetParameters().Select(parameter => new ParameterTarget(method, parameter)).ToArray();
		}
	}
}