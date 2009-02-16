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
using Ninject.Planning.Bindings;
#endregion

namespace Ninject
{
	/// <summary>
	/// Indicates that the decorated member should only be injected using binding(s) registered
	/// with the specified name.
	/// </summary>
	public class NamedAttribute : ConstraintAttribute
	{
		/// <summary>
		/// Gets the binding name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedAttribute"/> class.
		/// </summary>
		/// <param name="name">The name of the binding(s) to use.</param>
		public NamedAttribute(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Determines whether the specified binding metadata matches the constraint.
		/// </summary>
		/// <param name="metadata">The metadata in question.</param>
		/// <returns><c>True</c> if the metadata matches; otherwise <c>false</c>.</returns>
		public override bool Matches(IBindingMetadata metadata)
		{
			return metadata.Name == Name;
		}
	}
}
