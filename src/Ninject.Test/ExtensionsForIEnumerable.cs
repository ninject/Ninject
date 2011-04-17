namespace Ninject.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    
    public static class ExtensionsForIEnumerable
    {
        public static void Map<T>(this IEnumerable<T> series, Action<T> action)
        {
            foreach (T item in series)
            {
                action(item);
            }
        }

        public static T ShouldContainSingle<T>(this IEnumerable<T> source)
		{
            return Assert.Single(source);
		}
    }
}