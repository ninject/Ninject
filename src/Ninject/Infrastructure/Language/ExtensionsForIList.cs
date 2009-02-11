using System;
using System.Collections.Generic;

namespace Ninject.Infrastructure.Language
{
	internal static class ExtensionsForIList
	{
		public static bool ElementsEqual<T>(this IList<T> collection, IList<T> other)
		{
			if (collection.Count != other.Count)
				return false;

			for (int idx = 0; idx < collection.Count; idx++)
			{
				if (!collection[idx].Equals(other[idx]))
					return false;
			}

			return true;
		}
	}
}