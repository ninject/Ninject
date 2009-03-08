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
using System.Reflection;
#endregion

namespace Ninject.Infrastructure.Language
{
	internal static class ExtensionsForICustomAttributeProvider
	{
		public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider member)
			where T : Attribute
		{
			return member.GetCustomAttributes(typeof(T), true) as T[];
		}

		public static bool HasAttribute<T>(this ICustomAttributeProvider member)
			where T : Attribute
		{
			return member.IsDefined(typeof(T), true);
		}

		public static bool HasAttribute(this ICustomAttributeProvider member, Type type)
		{
			return member.IsDefined(type, true);
		}
	}
}