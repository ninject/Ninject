using System;
using System.Collections;
using System.Linq;

namespace Ninject.Infrastructure
{
	public static class LinqReflection
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