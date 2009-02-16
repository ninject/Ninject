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
using System.Collections;
using System.Linq;
#endregion

namespace Ninject.Infrastructure
{
	internal static class LinqReflection
	{
		public static object CastSlow(IEnumerable series, Type elementType)
		{
			var method = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(elementType);
			return method.Invoke(null, new[] { series });
		}

		public static Array ToArraySlow(IEnumerable series, Type elementType)
		{
			var method = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(elementType);
			return method.Invoke(null, new[] { series }) as Array;
		}

		public static IList ToListSlow(IEnumerable series, Type elementType)
		{
			var method = typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(elementType);
			return method.Invoke(null, new[] { series }) as IList;
		}
	}
}