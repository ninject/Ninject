using System;
using System.Collections.Generic;
using System.Linq;

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