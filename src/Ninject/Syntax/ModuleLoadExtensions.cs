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
using Ninject.Infrastructure;
using Ninject.Modules;
#endregion

namespace Ninject
{
	/// <summary>
	/// Extension methods that enhance module loading.
	/// </summary>
	public static class ModuleLoadExtensions
	{
		/// <summary>
		/// Creates a new instance of the module and loads it into the kernel.
		/// </summary>
		/// <typeparam name="TModule">The type of the module.</typeparam>
		public static void Load<TModule>(this IKernel kernel)
			where TModule : INinjectModule, new()
		{
			Ensure.ArgumentNotNull(kernel, "kernel");
			kernel.Load(new TModule());
		}
	}
}
