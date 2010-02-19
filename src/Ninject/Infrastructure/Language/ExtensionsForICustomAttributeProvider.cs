#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2010, Enkari, Ltd.
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 
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