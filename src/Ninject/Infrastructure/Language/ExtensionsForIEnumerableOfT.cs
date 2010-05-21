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
using System.Linq;
#endregion

namespace Ninject.Infrastructure.Language
{
	internal static class ExtensionsForIEnumerableOfT
	{
		public static void Map<T>(this IEnumerable<T> series, Action<T> action)
		{
			foreach (T item in series)
				action(item);
		}

		public static IEnumerable<T> ToEnumerable<T>(this IEnumerable<T> series)
		{
			return series.Select(x => x);
		}
	}
}