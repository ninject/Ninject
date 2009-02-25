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
#if !NO_WEB
using System.Web;
#endif
using Ninject.Activation;
#endregion

namespace Ninject.Infrastructure
{
	internal class StandardScopeCallbacks
	{
		public static readonly Func<IContext, object> Transient = ctx => null;
		public static readonly Func<IContext, object> Singleton = ctx => ctx.Kernel;
		public static readonly Func<IContext, object> Thread = ctx => System.Threading.Thread.CurrentThread;
		#if !NO_WEB
		public static readonly Func<IContext, object> Request = ctx => HttpContext.Current;
		#endif
	}
}