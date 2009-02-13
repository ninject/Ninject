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
using System.Reflection;
using Ninject.Activation;
using Ninject.Planning.Bindings;
#endregion

namespace Ninject.Planning.Targets
{
	/// <summary>
	/// Represents a site on a type where a value can be injected.
	/// </summary>
	public interface ITarget : ICustomAttributeProvider
	{
		/// <summary>
		/// Gets the type of the target.
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// Gets the name of the target.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the member that contains the target.
		/// </summary>
		MemberInfo Member { get; }

		/// <summary>
		/// Reads the constraints from the target.
		/// </summary>
		/// <returns>A series of constraints read from the target.</returns>
		IEnumerable<Func<IBindingMetadata, bool>> GetConstraints();

		/// <summary>
		/// Resolves a value for the target within the specified parent context.
		/// </summary>
		/// <param name="parent">The parent context.</param>
		/// <returns>The resolved value.</returns>
		object ResolveWithin(IContext parent);
	}
}