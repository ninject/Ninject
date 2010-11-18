namespace Ninject.Tests
{
    using System;
    using System.Collections.Generic;
#if SILVERLIGHT
    using System.Linq;
#if SILVERLIGHT_MSTEST
        using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
        using UnitDriven;
#endif
#else
    using Xunit;
#endif
    
    public static class ExtensionsForIEnumerable
    {
        public static void Map<T>(this IEnumerable<T> series, Action<T> action)
        {
            foreach (T item in series)
            {
                action(item);
            }
        }

#if SILVERLIGHT
        public static T ShouldContainSingle<T>(this IEnumerable<T> source)
		{
            try
            {
                return source.Single();
            }
            catch (InvalidOperationException e)
            {
                Assert.Fail("There is not exactly one element in the enumerable {0}, {1}", source, e.Message);
            }

            return default(T);
		}
#else
        public static T ShouldContainSingle<T>(this IEnumerable<T> source)
		{
            return Assert.Single(source);
		}
#endif
    }
}