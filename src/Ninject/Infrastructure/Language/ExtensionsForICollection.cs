#region License
// 
// Author: Nate Kohari <nate@enkari.com>
// Copyright (c) 2007-2009, Enkari, Ltd.
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
	internal static class ExtensionsForICollection
	{
		public static void RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> condition)
		{
			collection.Where(condition).ToArray().Map(item => collection.Remove(item));
		}
	}
}