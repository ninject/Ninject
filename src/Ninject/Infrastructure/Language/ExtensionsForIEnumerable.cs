using System;
using System.Collections.Generic;

namespace Ninject.Infrastructure.Language
{
	internal static class ExtensionsForIEnumerable
	{
		public static void Map<T>(this IEnumerable<T> series, Action<T> action)
		{
			foreach (T item in series)
				action(item);
		}
	}
}