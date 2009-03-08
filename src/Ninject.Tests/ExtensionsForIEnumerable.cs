using System;
using System.Collections.Generic;

namespace Ninject.Tests
{
	public static class ExtensionsForIEnumerable
	{
		public static void Map<T>(this IEnumerable<T> series, Action<T> action)
		{
			foreach (T item in series)
				action(item);
		}
	}
}