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

namespace Ninject.Planning.Targets
{
	/// <summary>
	/// Represents an injection target for a <see cref="PropertyInfo"/>.
	/// </summary>
	public class PropertyTarget : Target<PropertyInfo>
	{
		/// <summary>
		/// Gets the name of the target.
		/// </summary>
		public override string Name
		{
			get { return Site.Name; }
		}

		/// <summary>
		/// Gets the type of the target.
		/// </summary>
		public override Type Type
		{
			get { return Site.PropertyType; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyTarget"/> class.
		/// </summary>
		/// <param name="site">The property that this target represents.</param>
		public PropertyTarget(PropertyInfo site) : base(site, site) { }
	}
}