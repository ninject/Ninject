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
using Ninject.Planning.Targets;
#endregion

namespace Ninject.Planning.Directives
{
	/// <summary>
	/// Describes the injection of a property.
	/// </summary>
	public class PropertyInjectionDirective : IDirective
	{
		/// <summary>
		/// Gets or sets the member the directive describes.
		/// </summary>
		public PropertyInfo Member { get; private set; }

		/// <summary>
		/// Gets or sets the injection target for the directive.
		/// </summary>
		public ITarget Target { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyInjectionDirective"/> class.
		/// </summary>
		/// <param name="member">The member the directive describes.</param>
		public PropertyInjectionDirective(PropertyInfo member)
		{
			Member = member;
			Target = new PropertyTarget(member);
		}
	}
}