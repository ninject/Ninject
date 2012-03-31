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
using System.Collections;
using System.Linq;

#if WINRT
using System.Reflection;
#endif
#endregion

namespace Ninject.Infrastructure.Language
{
    internal static class ExtensionsForIEnumerable
    {
        public static IEnumerable CastSlow(this IEnumerable series, Type elementType)
        {
#if WINRT
            var method = typeof(Enumerable).GetTypeInfo().GetDeclaredMethod("Cast").MakeGenericMethod(elementType);
#else
            var method = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(elementType);
#endif
            return method.Invoke(null, new[] { series }) as IEnumerable;
        }

        public static Array ToArraySlow(this IEnumerable series, Type elementType)
        {
#if WINRT
            var method = typeof(Enumerable).GetTypeInfo().GetDeclaredMethod("ToArray").MakeGenericMethod(elementType);
#else
            var method = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(elementType);
#endif

            return method.Invoke(null, new[] { series }) as Array;
        }

        public static IList ToListSlow(this IEnumerable series, Type elementType)
        {
#if WINRT
            var method = typeof(Enumerable).GetTypeInfo().GetDeclaredMethod("ToList").MakeGenericMethod(elementType);
#else
            var method = typeof(Enumerable).GetMethod("ToList").MakeGenericMethod(elementType);
#endif
            return method.Invoke(null, new[] { series }) as IList;
        }
    }
}