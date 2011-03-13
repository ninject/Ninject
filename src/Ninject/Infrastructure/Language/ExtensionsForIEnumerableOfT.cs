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

        public static Maybe<T> FirstOrNone<T>(this IEnumerable<Maybe<T>> series, Func<T, bool> predicate)
        {
            using (var e = series.Where(x => x.HasValue).Where(x => predicate(x.Value)).GetEnumerator())
            {
                return e.MoveNext() ? e.Current : Maybe<T>.None;
            }
        }

        public static Maybe<T> FirstOrNone<T>(this IEnumerable<Maybe<T>> series)
        {
            return series.FirstOrNone<T>(_ => true);
        }

        public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> series, Func<T, bool> predicate)
        {
            return series.Select(x => new Maybe<T>(x)).FirstOrNone(predicate);
        }

        public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> series)
        {
            return series.Select(x => new Maybe<T>(x)).FirstOrNone();
        }
    }
}