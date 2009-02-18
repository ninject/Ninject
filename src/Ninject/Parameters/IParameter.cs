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
using Ninject.Activation;
#endregion

namespace Ninject.Parameters
{
	/// <summary>
	/// Modifies an activation process in some way.
	/// </summary>
	public interface IParameter : IEquatable<IParameter>
	{
		/// <summary>
		/// Gets the name of the parameter.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a value indicating whether the parameter should be inherited into child requests.
		/// </summary>
		bool ShouldInherit { get; }

		/// <summary>
		/// Gets the value for the parameter within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The value for the parameter.</returns>
		object GetValue(IContext context);
	}
}