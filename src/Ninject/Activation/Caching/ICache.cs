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
using Ninject.Components;
#endregion

namespace Ninject.Activation.Caching
{
	/// <summary>
	/// Tracks instances for re-use in certain scopes.
	/// </summary>
	public interface ICache : INinjectComponent
	{
		/// <summary>
		/// Stores the specified context in the cache.
		/// </summary>
		/// <param name="context">The context to store.</param>
		void Remember(IContext context);

		/// <summary>
		/// Tries to retrieve an instance to re-use in the specified context.
		/// </summary>
		/// <param name="context">The context that is being activated.</param>
		/// <returns>The instance for re-use, or <see langword="null"/> if none has been stored.</returns>
		object TryGet(IContext context);

		/// <summary>
		/// Removes instances from the cache which should no longer be re-used.
		/// </summary>
		void Prune();
	}
}