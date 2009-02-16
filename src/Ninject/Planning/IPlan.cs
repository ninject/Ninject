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
using Ninject.Planning.Directives;
#endregion

namespace Ninject.Planning
{
	/// <summary>
	/// Describes the means by which a type should be activated.
	/// </summary>
	public interface IPlan
	{
		/// <summary>
		/// Gets the type that the plan describes.
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// Adds the specified directive to the plan.
		/// </summary>
		/// <param name="directive">The directive.</param>
		void Add(IDirective directive);

		/// <summary>
		/// Determines whether the plan contains one or more directives of the specified type.
		/// </summary>
		/// <typeparam name="TDirective">The type of directive.</typeparam>
		/// <returns><c>True</c> if the plan has one or more directives of the type; otherwise, <c>false</c>.</returns>
		bool Has<TDirective>() where TDirective : IDirective;

		/// <summary>
		/// Gets the first directive of the specified type from the plan.
		/// </summary>
		/// <typeparam name="TDirective">The type of directive.</typeparam>
		/// <returns>The first directive, or <see langword="null"/> if no matching directives exist.</returns>
		TDirective GetOne<TDirective>() where TDirective : IDirective;

		/// <summary>
		/// Gets all directives of the specified type that exist in the plan.
		/// </summary>
		/// <typeparam name="TDirective">The type of directive.</typeparam>
		/// <returns>A series of directives of the specified type.</returns>
		IEnumerable<TDirective> GetAll<TDirective>() where TDirective : IDirective;
	}
}