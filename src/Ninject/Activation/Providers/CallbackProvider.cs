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
#endregion

namespace Ninject.Activation.Providers
{
	/// <summary>
	/// A provider that delegates to a callback method to create instances.
	/// </summary>
	/// <typeparam name="T">The type of instances the provider creates.</typeparam>
	public class CallbackProvider<T> : Provider<T>
	{
		/// <summary>
		/// Gets the callback method used by the provider.
		/// </summary>
		public Func<IContext, T> Method { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CallbackProvider&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="method">The callback method that will be called to create instances.</param>
		public CallbackProvider(Func<IContext, T> method)
		{
			Method = method;
		}

		/// <summary>
		/// Invokes the callback method to create an instance.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>The created instance.</returns>
		protected override T CreateInstance(IContext context)
		{
			return Method(context);
		}
	}
}